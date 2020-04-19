using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Radiology.Patch
{
    [HarmonyPatch(typeof(MedicalRecipesUtility), "IsCleanAndDroppable")]
    class PatchMedicalRecipesUtilityIsCleanAndDroppable
    {
        static bool Prefix(ref bool __result, Pawn pawn, BodyPartRecord part)
        {
            if (!pawn.Dead && !pawn.RaceProps.Animal && HasAnyMutations(pawn, part) && Radiology.bodyPartItems.ContainsKey(part.def))
            {
                __result = true;
                return false;
            }

            return true;
        }

        public static bool HasAnyMutations(Pawn pawn, BodyPartRecord part)
        {
            return (from x in pawn.health.hediffSet.hediffs.OfType<Mutation>() where x.Part == part && !x.def.isBad select x).Any();
        }
    }

    [HarmonyPatch(typeof(MedicalRecipesUtility), "SpawnNaturalPartIfClean")]
    class PatchMedicalRecipesUtilitySpawnNaturalPartIfClean
    {

        static bool Prefix(ref Thing __result, Pawn pawn, BodyPartRecord part, IntVec3 pos, Map map)
        {
            if (!PatchMedicalRecipesUtilityIsCleanAndDroppable.HasAnyMutations(pawn, part)) return true;

            ThingDef thingDef;
            if (!Radiology.bodyPartItems.TryGetValue(part.def, out thingDef)) return true;

            Thing thing = GenSpawn.Spawn(thingDef, pos, map, WipeMode.Vanish);
            CompHediffStorage comp = thing.TryGetComp<CompHediffStorage>();
            if (comp != null) {
                comp.parts.Clear();
                comp.hediffs.Clear();

                foreach (Hediff hediff in pawn.health.hediffSet.hediffs.Where(x => HealthHelper.IsParent(part, x.Part)))
                {
                    comp.parts.Add(hediff.Part.def);
                    comp.hediffs.Add(hediff);
                }
            }

            __result = thing;
            return false;
        }

    }


}
