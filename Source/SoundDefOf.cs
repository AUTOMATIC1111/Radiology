using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    [DefOf]
    public static class SoundDefOf
    {
        static SoundDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(SoundDefOf));
        }

        public static SoundDef RadiologyIrradiateBasic;

        public static object Click { get; internal set; }
    }

}
