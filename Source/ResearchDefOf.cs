using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    [DefOf]
    public static class ResearchDefOf
    {
        static ResearchDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ResearchDefOf));
        }

        public static ResearchProjectDef RadiologyFilteringIntermediate;
        public static ResearchProjectDef RadiologyFilteringTradeoff;
        public static ResearchProjectDef RadiologyFilteringAdvanced;
    }

}
