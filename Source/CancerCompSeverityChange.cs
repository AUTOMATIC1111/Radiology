using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiology
{
    public class CancerCompSeverityChangeDef : CancerCompDef
    {
        public CancerCompSeverityChangeDef()
        {
            Init(typeof(CancerCompSeverityChange));
        }

        public float changePerDay;
    }

    public class CancerCompSeverityChange : CancerComp<CancerCompSeverityChangeDef>
    {
        public override object[] DescriptionArgs => null;

        public override void Update(int passed) => cancer.Severity += passed * def.changePerDay / 60000;
    }
}
