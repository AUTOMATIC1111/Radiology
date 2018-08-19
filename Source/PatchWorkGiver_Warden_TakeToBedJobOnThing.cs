using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace Radiology
{
    /// <summary>
    /// Prevent pawns from equipping something if they have mutated body part apparel
    /// </summary>
    [HarmonyPatch(typeof(WorkGiver_Warden_TakeToBed), "JobOnThing", new Type[] { typeof(Pawn), typeof(Thing), typeof(bool) }), StaticConstructorOnStartup]
    public static class PatchWorkGiver_Warden_TakeToBedJobOnThing
    {
        static bool Prefix(Pawn pawn, Thing t, bool forced, ref Job __result)
        {
            Pawn prisoner = t as Pawn;
            if (prisoner == null) return true;

            if (forced) return true;

            if (prisoner.CurJob != null && prisoner.CurJob.GetCachedDriver(prisoner) is IPrisonerAllowedJob)
            {
                __result = null;
                return false;
            }

            return true;
        }
    }
}
