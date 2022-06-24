using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    public class RadiationInfo
    {
        public ChamberDef chamberDef;
        public Pawn pawn;
        public BodyPartRecord part;
        public bool secondHand;

        public float burn;
        public float normal;
        public float rare;

        public HashSet<CompIrradiator> visited;

        public void Add(RadiationInfo info)
        {
            burn += info.burn;
            normal += info.normal;
            rare += info.rare;
        }

        public bool Empty()
        {
            return burn <= 0 && normal <= 0 && rare <= 0;
        }

        public void CopyFrom(RadiationInfo other)
        {
            chamberDef = other.chamberDef;
            pawn = other.pawn;
            part = other.part;
            secondHand = other.secondHand;
            visited = other.visited;
        }
    }
}
