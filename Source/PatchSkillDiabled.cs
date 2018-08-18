using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace Radiology
{
    /// <summary>
    /// hook to check if a mutation disables a skill
    /// </summary>
    [HarmonyPatch(typeof(SkillRecord), "CalculateTotallyDisabled", new Type[] { }), StaticConstructorOnStartup]
    public static class PatchSkillDiabled
    {
        static private FieldInfo pawnField = typeof(SkillRecord).GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance);

        static void Postfix(ref bool __result, SkillRecord __instance)
        {
            Pawn pawn = pawnField.GetValue(__instance) as Pawn;
            foreach (var v in pawn.health.hediffSet.GetHediffs<MutationSetSkill>())
            {
                __result = v.IsSkillDisabled(__instance.def);
                return;
            }
        }
    }
}
