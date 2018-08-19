using RimWorld;
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

        public static bool IsShielded(this Pawn pawn)
        {
            if (pawn == null || pawn.apparel == null) return false;

            DamageInfo damageTest = new DamageInfo(DamageDefOf.Bomb, 0f, 0f, -1, null);
            foreach (Apparel apparel in pawn.apparel.WornApparel)
            {
                if (apparel.CheckPreAbsorbDamage(damageTest)) return true;
            }

            return false;
        }
    }
}
