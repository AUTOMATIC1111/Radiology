using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace Radiology
{
    public class WorkGiverIrradiatePrisoner : WorkGiver_Warden
    {
        static string reasonSleeping = "ChamberSleeping";
        static string reasonDowned = "ChamberDowned";
        static string reasonPrisonerInaccessible = "ChamberPrisonerInaccessible";

        bool CheckJob(Pawn pawn, Thing t, bool forced, out Chamber chamberRef, ref string reason)
        {
            Pawn prisoner = t as Pawn;
            chamberRef = null;

            if (!prisoner.IsPrisoner) return false;

            if (!prisoner.Awake())
            {
                reason = reasonSleeping;
                return false;
            }

            if (prisoner.Downed)
            {
                reason = reasonDowned;
                return false;
            }

            if (!ShouldTakeCareOfPrisoner(pawn, prisoner))
            {
                reason = reasonPrisonerInaccessible;
                return false;
            }

            if (prisoner.CurJob != null && prisoner.CurJob.GetCachedDriver(prisoner) is IPrisonerAllowedJob) return false;

            foreach (Chamber chamber in pawn.Map.listerBuildings.AllBuildingsColonistOfClass<Chamber>())
            {
                string currentReason = chamber.CanIrradiateNow(prisoner);
                if (currentReason == null && pawn.CanReserve(chamber, 1, -1, null, forced))
                {
                    chamberRef = chamber;
                    return true;
                }

                reason = reason ?? currentReason;
            }

            return false;
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Chamber chamber;
            string reason = null;

            bool res = CheckJob(pawn, t, forced, out chamber, ref reason);
            if (!res && reason != null)
            {
                JobFailReason.Is(string.Format(reason.Translate(), t.LabelShortCap), null);
            }

            return res;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Chamber chamber;
            string reason = null;
            if (!CheckJob(pawn, t, forced, out chamber, ref reason)) return null;

            return new Job(JobDefOf.RadiologyIrradiatePrisoner, t, chamber) { count = 1 };
        }
    }
}
