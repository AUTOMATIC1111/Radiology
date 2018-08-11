using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{

    public class ChamberDef : ThingDef
    {
        public FloatRange burnThreshold;
        public FloatRange mutateThreshold;
    }
}
