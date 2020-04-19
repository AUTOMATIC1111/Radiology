using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;

namespace Radiology
{
    public class MutationBloodHealerDef : MutationDef
    {
        public bool healMissingParts = false;

        public RadiologyEffectSpawnerDef effectRegeneration;

        public void Heal(Pawn pawn, Pawn target, DamageInfo dinfo, float totalDamageDealt)
        {
            RegenerateInjury(pawn, totalDamageDealt * 0.25f);
        }

        public bool RegenerateInjury(Pawn pawn, float amount)
        {
            var injuries = pawn.health.hediffSet.hediffs.OfType<Hediff_Injury>().Where(x => x.Severity > 0 && x.Part != null);
            Hediff_Injury injury = injuries.RandomElementWithFallback();
            if (injury == null) return false;

            HediffComp_GetsPermanent hediffComp_GetsPermanent = injury.TryGetComp<HediffComp_GetsPermanent>();
            if (hediffComp_GetsPermanent != null) hediffComp_GetsPermanent.IsPermanent = false;

            injury.Severity = Mathf.Max(injury.Severity - amount);
            pawn.health.hediffSet.DirtyCache();

            RadiologyEffectSpawnerDef.Spawn(effectRegeneration, pawn);
            return true;
        }
    }
}
