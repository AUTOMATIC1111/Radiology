using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    /// <summary>
    /// Forbid dropping of mutation apparel items.
    /// </summary>
    [HarmonyPatch(typeof(Pawn_ApparelTracker), "TryDrop", new Type[] { typeof(Apparel), typeof(Apparel), typeof(IntVec3), typeof(bool) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Out, ArgumentType.Normal, ArgumentType.Normal }), StaticConstructorOnStartup]
    public static class PatchApparelTrackerTryDrop
    {
        static bool Prefix(ref Apparel ap, out Apparel resultingAp, IntVec3 pos, bool forbid, ref bool __result)
        {
            resultingAp = null;

            ApparelBodyPart apparel = ap as ApparelBodyPart;
            if (apparel == null) return true;

            __result = false;
            return false;
        }
    }
}
