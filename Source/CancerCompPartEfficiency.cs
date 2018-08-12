using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    public class CancerCompPartEfficiencyDef : CancerCompDef
    {
        public CancerCompPartEfficiencyDef()
        {
            Init(typeof(CancerCompPartEfficiency));
        }

        public float offset;
    }

    public class CancerCompPartEfficiency : CancerComp<CancerCompPartEfficiencyDef>
    {
        public override object[] DescriptionArgs => new object[] { (int)(-Offset * 100) };

        public float Offset => def.offset * cancer.Severity;

        public override void Update(int passed) => cancer.stage.partEfficiencyOffset = Offset;
    }
}
