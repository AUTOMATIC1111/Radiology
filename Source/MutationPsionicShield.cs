using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    public class CompPsionicShield : ThingComp
    {
        public MutationPsionicShield mutation;

        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            mutation.ApplyDamage(dinfo, out absorbed);
        }
    }

    public class MutationPsionicShieldDef : HediffMutationDef
    {
        public MutationPsionicShieldDef(){ hediffClass = typeof(MutationPsionicShield); }

        public float health;
        public float regenratedPerSecond;

        public int regenerationDelayTicks;

        public AutomaticEffectSpawnerDef effectAbsorbed;
        public AutomaticEffectSpawnerDef effectBroken;
        public AutomaticEffectSpawnerDef effectRestored;
    }

    public class MutationPsionicShield : Mutation<MutationPsionicShieldDef>
    {
        public override ThingComp[] GetComps()
        {
            return new ThingComp[] { new CompPsionicShield() { mutation = this } };
        }

        public void ApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            if (dinfo.Amount <= health)
            {
                health -= dinfo.Amount;
                absorbed = true;

                AutomaticEffectSpawnerDef.Spawn(def.effectAbsorbed, pawn, dinfo.Angle);

                return;
            }

            dinfo.SetAmount(dinfo.Amount - health);
            health = 0;
            regenerationDelay = def.regenerationDelayTicks;
            absorbed = false;

            AutomaticEffectSpawnerDef.Spawn(def.effectBroken, pawn);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref health, "health");
            Scribe_Values.Look(ref regenerationDelay, "regenerationDelay");
        }

        public override void Tick()
        {
            base.Tick();

            if (regenerationDelay > 0)
            {
                regenerationDelay--;
            }
            else
            {
                if (health == 0 && def.regenratedPerSecond != 0)
                {
                    AutomaticEffectSpawnerDef.Spawn(def.effectRestored, pawn);
                }

                health += def.regenratedPerSecond / 60;
            }
        }

        float health = 0;
        int regenerationDelay = 0;
    }

}
