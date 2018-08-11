using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace Radiology
{
    static class HelperCompFacility
    {
        static private FieldInfo linkedBuildingsField = null;
        public static List<Thing> LinkedBuildings(this CompFacility facility)
        {
            if (linkedBuildingsField == null)
            {
                linkedBuildingsField = typeof(CompFacility).GetField("linkedBuildings", BindingFlags.NonPublic | BindingFlags.Instance);
            }

            return linkedBuildingsField.GetValue(facility) as List<Thing>;
        }

        public static T Linked<T>(this CompFacility facility) where T : Thing
        {
            foreach (var thing in facility.LinkedBuildings())
            {
                T res = thing as T;
                if (res != null) return res;
            }

            return null;
        }

        public static T Linked<T>(this ThingWithComps thing) where T : Thing
        {
            CompFacility comp = thing.GetComp<CompFacility>();
            if(comp==null) return null;

            return comp.Linked<T>();
        }

    }
}
