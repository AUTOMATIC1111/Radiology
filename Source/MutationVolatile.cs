using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    public class MutationVolatileDef : MutationDef
    {
        public MutationVolatileDef()
        {
            hediffClass = typeof(MutationVolatile);
        }
    }

    public class MutationVolatile : Mutation<MutationVolatileDef>
    {
        float healthy = -1;

        public override void Tick()
        {
            base.Tick();

            if (!pawn.IsHashIntervalTick(60)) return;

            float health = pawn.health.hediffSet.GetPartHealth(Part);
            if (health > healthy) healthy = health;

            if (health < healthy)
            {
                GenExplosion.DoExplosion(pawn.Position, pawn.Map, 6, DamageDefOf.Bomb, pawn);
                healthy = health;
            }
        }
    }
}
