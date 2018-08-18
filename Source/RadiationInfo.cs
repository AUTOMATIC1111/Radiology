using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    public class RadiationInfo
    {
        public Chamber chamber;
        public Pawn pawn;
        public BodyPartRecord part;

        public float burn;
        public float normal;
        public float rare;
        public bool secondHand;
    }     
}
