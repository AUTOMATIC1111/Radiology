using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology.Patch
{
    /// <summary>
    /// Forbid dropping of mutation apparel items.
    /// </summary>
    [HarmonyPatch(typeof(Pawn_ApparelTracker), "TryDrop", new Type[] { typeof(Apparel), typeof(Apparel), typeof(IntVec3), typeof(bool) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Out, ArgumentType.Normal, ArgumentType.Normal })]
    public static class PatchApparelTrackerTryDrop
    {
        static bool Prefix(ref Apparel ap, out Apparel resultingAp, IntVec3 pos, bool forbid, ref bool __result, Pawn_ApparelTracker __instance)
        {
            resultingAp = null;

            ApparelBodyPart apparel = ap as ApparelBodyPart;
            if (apparel == null) return true;

            Pawn pawn = __instance.pawn;
            Outfit currentOutfit = pawn.outfits.CurrentOutfit;
            currentOutfit.filter.SetAllow(ap.def, true);

            __result = false;
            return false;
        }
    }
}
