using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;
using Verse.AI;

namespace Radiology
{
    [HarmonyPatch(typeof(JobDriver_Vomit), "MakeNewToils", new Type[] { }), StaticConstructorOnStartup]
    public static class PatchVomiting
    {
        static private FieldInfo ticksLeft = typeof(JobDriver_Vomit).GetField("ticksLeft", BindingFlags.NonPublic | BindingFlags.Instance);

        static IEnumerable<Toil> Postfix(IEnumerable<Toil> list, JobDriver_Vomit __instance)
        {
            IEnumerable<MutationVomitItems> mutations = __instance.pawn.health.hediffSet.GetHediffs<MutationVomitItems>();

            foreach (Toil toil in list)
            {
                if(mutations.Any())
                {
                    Action act = toil.tickAction;
                    toil.tickAction = delegate ()
                    {
                        if (((int)ticksLeft.GetValue(__instance)) % 150 == 149)
                        {
                            foreach (MutationVomitItems mutation in mutations)
                            {
                                mutation.Vomiting(__instance.job.targetA.Cell);
                            }
                        }

                        act();
                    };
                }


                yield return toil;
            }

            yield break;
        }

    }

}
