using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    public class CancerCompCapacityOffsetDef : CancerCompDef
    {
        public CancerCompCapacityOffsetDef()
        {
            Init(typeof(CancerCompCapacityOffset));
        }

        public float offset;
        public PawnCapacityDef capacity;
    }

    public class CancerCompCapacityOffset : CancerComp<CancerCompCapacityOffsetDef>
    {
        public override object[] DescriptionArgs => new object[] { def.capacity.label, (int) (-Offset * 100) };

        public float Offset => def.offset * cancer.Severity;

        public override void Update(int passed) => cancer.stage.capMods.Add( new PawnCapacityModifier() { capacity = def.capacity, offset = Offset } );
    }
}
