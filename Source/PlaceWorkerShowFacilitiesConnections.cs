using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Radiology
{
    public class PlaceWorkerShowFacilitiesConnections : PlaceWorker
    {
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol)
        {
            Map currentMap = Find.CurrentMap;
            if (def.HasComp(typeof(CompAffectedByFacilities)))
            {
                CompAffectedByFacilities.DrawLinesToPotentialThingsToLinkTo(def, center, rot, currentMap);
            }

            if (def.HasComp(typeof(CompFacility)))
            {
                CompFacility.DrawLinesToPotentialThingsToLinkTo(def, center, rot, currentMap);
            }
        }
    }
}
