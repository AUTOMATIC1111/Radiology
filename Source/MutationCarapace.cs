using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Radiology
{
    public class CompCarapace : ThingComp
    {
        public MutationCarapace mutation;

        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            mutation.ApplyDamage(dinfo, out absorbed);
        }
    }

    public class MutationCarapaceDef : HediffMutationDef
    {
        public MutationCarapaceDef()
        {
            hediffClass = typeof(MutationCarapace);
        }

        public float ratio;
        public AutomaticEffectSpawnerDef effectReflect;
    }

    public class MutationCarapace : Mutation<MutationCarapaceDef>
    {
        public override ThingComp[] GetComps()
        {
            return new ThingComp[] { new CompCarapace() { mutation = this } };
        }

        public void ApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            if (Mathf.Abs(MathHelper.AngleDifference(dinfo.Angle, pawn.Rotation.AsAngle)) < 45)
            {
                absorbed = false;
                return;
            }

            AutomaticEffectSpawnerDef.Spawn(def.effectReflect, pawn, dinfo.Angle);
            dinfo.SetAmount(dinfo.Amount * def.ratio);
            absorbed = false;
        }
    }
}
