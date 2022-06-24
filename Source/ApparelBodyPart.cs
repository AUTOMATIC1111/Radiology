using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiology
{
    class ApparelBodyPart :Apparel
    {
        public override float GetSpecialApparelScoreOffset()
        {
            return 10000f;
        }
    }
}
