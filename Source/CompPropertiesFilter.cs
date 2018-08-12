using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    public class CompPropertiesFilter :CompProperties
    {
        public CompPropertiesFilter()
        {
            compClass = typeof(CompFilter);
        }
    }
}
