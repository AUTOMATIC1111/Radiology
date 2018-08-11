using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    public class CompPropertiesBlocker : CompProperties
    {
        public CompPropertiesBlocker()
        {
            compClass = typeof(CompBlocker);
        }

        public int blockedBodyPartLimit = 2;
        public float blockChance = 0.75f;
    }
}
