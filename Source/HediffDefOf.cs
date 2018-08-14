using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    [DefOf]
    public static class HediffDefOf
    {
        static HediffDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(HediffDefOf));
        }

        public static HediffDef RadiologyRadiation;
        public static HediffMutationDef MutationSuperSpeed;
        public static CancerDef RadiologyCancer;
    }
}
