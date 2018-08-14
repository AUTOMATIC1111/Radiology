using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace Radiology
{
    public class WorkGiverIrradiate : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(ThingDefOf.RadiologyRadiationChamber);

        public override PathEndMode PathEndMode => PathEndMode.InteractionCell;

        public override Danger MaxPathDanger(Pawn pawn)
        {
            return Danger.Deadly;
        }

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            return pawn.Map.listerBuildings.AllBuildingsColonistOfDef(ThingDefOf.RadiologyRadiationChamber).Cast<Thing>();
        }

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            if (forced) return false;

            foreach (Chamber chamber in pawn.Map.listerBuildings.AllBuildingsColonistOfClass<Chamber>())
            {
                if (chamber.CanIrradiateNow() == null)
                {
                    return false;
                }
            }
            return true;
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t.Faction != pawn.Faction) return false;

            Chamber chamber = t as Chamber;
            if (chamber == null) return false;

            string reason = chamber.CanIrradiateNow(pawn);
            if (reason != null)
            {
                JobFailReason.Is(string.Format(reason.Translate(), pawn.LabelShortCap), null);
                return false;
            }

            if (chamber.IsForbidden(pawn)) return false;
 
            LocalTargetInfo target = chamber;
            if (!pawn.CanReserve(target, 1, -1, null, forced)) return false;
            
            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return new Job(JobDefOf.RadiologyIrradiate, t, 1500, true);
        }
    }
}
