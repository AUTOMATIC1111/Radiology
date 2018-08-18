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

    public class MutationCarapaceDef : MutationDef
    {
        public MutationCarapaceDef()
        {
            hediffClass = typeof(MutationCarapace);
        }

        public float ratio;
        public RadiologyEffectSpawnerDef effectReflect;
    }

    public class MutationCarapace : Mutation<MutationCarapaceDef>
    {
        public override ThingComp[] GetComps()
        {
            return new ThingComp[] { new CompCarapace() { mutation = this } };
        }

        public void ApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            absorbed = false;

            if (Mathf.Abs(MathHelper.AngleDifference(dinfo.Angle + 180, pawn.Rotation.AsAngle)) < 45)
            {
                return;
            }

            RadiologyEffectSpawnerDef.Spawn(def.effectReflect, pawn, dinfo.Angle + 180);
            dinfo.SetAmount(dinfo.Amount * def.ratio);
        }
    }
}
