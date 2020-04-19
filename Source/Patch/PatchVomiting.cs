using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;
using Verse.AI;

namespace Radiology.Patch
{
    /// <summary>
    /// Hook to call vomiting mutation when the pawn is vomiting
    /// </summary>
    [HarmonyPatch(typeof(JobDriver_Vomit), "MakeNewToils", new Type[] { })]
    public static class PatchVomiting
    {
        static private FieldInfo ticksLeft = typeof(JobDriver_Vomit).GetField("ticksLeft", BindingFlags.NonPublic | BindingFlags.Instance);

        static IEnumerable<Toil> Postfix(IEnumerable<Toil> list, JobDriver_Vomit __instance)
        {
            IEnumerable<MutationVomitItems> mutations = __instance.pawn.health.hediffSet.GetHediffs<MutationVomitItems>();
            if (!mutations.Any()) return list;

            foreach (Toil toil in list)
            {
                toil.AddFinishAction(delegate ()
                {
                    foreach (MutationVomitItems mutation in mutations)
                    {
                        mutation.Vomiting(__instance.job.targetA.Cell);
                    }
                });
            }

            return list;
        }

    }

}
