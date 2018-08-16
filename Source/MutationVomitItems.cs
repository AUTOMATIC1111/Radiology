using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    public class MutationVomitItemsDef : HediffMutationDef
    {
        public MutationVomitItemsDef() { hediffClass = typeof(MutationVomitItems); }

        public ThingDef item;
        public IntRange count;
        public FloatRange damage;

        public List<BodyPartDef> hurtParts;
        public float hurtChance;

        public AutomaticEffectSpawnerDef effect;
    }

    public class MutationVomitItems : Mutation<MutationVomitItemsDef>
    {
        public virtual void Vomiting(IntVec3 cell)
        {
            if (def.item == null) return;

            Thing thing = ThingMaker.MakeThing(def.item, null);
            thing.stackCount = def.count.RandomInRange;
            GenPlace.TryPlaceThing(thing, cell, pawn.Map, ThingPlaceMode.Direct, null, null);

            AutomaticEffectSpawnerDef.Spawn(def.effect, pawn);

            if (def.hurtParts == null) return;

            foreach (BodyPartDef partDef in def.hurtParts)
            {
                foreach (BodyPartRecord part in pawn.health.hediffSet.GetNotMissingParts().Where(x => def.hurtParts.Contains(x.def)))
                {
                    if (Rand.Value > def.hurtChance) continue;

                    DamageInfo dinfo = new DamageInfo(DamageDefOf.Cut, def.damage.RandomInRange, 999999f, -1f, thing, part, null, DamageInfo.SourceCategory.ThingOrUnknown, null);
                    pawn.TakeDamage(dinfo);
                }
            }
        }
    }
}
