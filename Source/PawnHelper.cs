using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace Radiology
{
    [StaticConstructorOnStartup]
    public static class PawnHelper
    {
        static private FieldInfo compsField = typeof(ThingWithComps).GetField("comps", BindingFlags.NonPublic | BindingFlags.Instance);

        public static List<ThingComp> Comps(this ThingWithComps thing)
        {
            return compsField.GetValue(thing) as List<ThingComp>;
        }
    }
}
