using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    [HarmonyPatch(typeof(Pawn), "GetGizmos", new Type[] { }), StaticConstructorOnStartup]
    public static class PatchHediffGizmos
    {
        /* ???
        static void Prefix(ref IEnumerable<Gizmo> __result, ref Pawn __instance)
        {
            Mutation self = GetCompProperties(facilityDef);

                yield break;

        }
        */
    }

}
