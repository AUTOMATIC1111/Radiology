using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    /// <summary>
    /// Allows hediffs to add gizmos (should really be removed and re-implemented using existing comps mechanism)
    /// </summary>
    [HarmonyPatch(typeof(Pawn), "GetGizmos", new Type[] { }), StaticConstructorOnStartup]
    public static class PatchHediffGizmos
    {

        static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> list, Pawn __instance)
        {
            foreach (var v in list)
                yield return v;

            foreach (var mutation in __instance.health.hediffSet.GetHediffs<Mutation>())
            {
                foreach (var gizmo in mutation.GetGizmos())
                {
                    yield return gizmo;
                }
            }

            yield break;

        }
    }
}
