using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    /// <summary>
    /// Let a nuilding be both facility and affected by facilities when placing it.
    /// </summary>
    [HarmonyPatch(typeof(CompAffectedByFacilities), "CanPotentiallyLinkTo_Static", new Type[] { typeof(ThingDef), typeof(IntVec3), typeof(Rot4), typeof(ThingDef), typeof(IntVec3), typeof(Rot4) }), StaticConstructorOnStartup]
    public static class PatchFacilities
    {

        static public IAdvancedFacilityConnector GetCompProperties(ThingDef def)
        {
            for (int i = 0; i < def.comps.Count; i++)
            {
                IAdvancedFacilityConnector t = def.comps[i] as IAdvancedFacilityConnector;
                if (t != null)
                {
                    return t;
                }
            }
            return null;
        }

        static void Postfix(ref ThingDef facilityDef, ref IntVec3 facilityPos, ref Rot4 facilityRot, ref ThingDef myDef, ref IntVec3 myPos, ref Rot4 myRot, ref bool __result)
        {
            IAdvancedFacilityConnector self = GetCompProperties(facilityDef);
            if (self == null) self = GetCompProperties(myDef); ;
            if (self == null) return;

            bool res = self.CanLinkTo(__result, facilityDef, facilityPos, facilityRot, myDef, myPos, myRot);
            __result = res;
        }
    }
}
