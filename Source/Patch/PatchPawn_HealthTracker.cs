using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Radiology.Patch
{
    
    [HarmonyPatch(typeof(Pawn_HealthTracker), "PostApplyDamage")]
    class PatchPawn_HealthTracker
    {
        static void Postfix(Pawn_HealthTracker __instance, DamageInfo dinfo, float totalDamageDealt)
        {
            MutationBloodHealerDef def = dinfo.WeaponLinkedHediff as MutationBloodHealerDef;
            if (def == null) return;

            Pawn pawn = dinfo.Instigator as Pawn;
            if (pawn == null) return;

            Pawn target = __instance.hediffSet.pawn;
            if (target?.RaceProps == null || !target.RaceProps.IsFlesh) return;

            def.Heal(pawn, target, dinfo, totalDamageDealt);
        }
    }
}
