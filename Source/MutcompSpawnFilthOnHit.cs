using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Radiology
{
    public class MutcompSpawnFilthOnHitProps : MutcompProps<MutcompSpawnFilthOnHit>
    {
        public ThingDef filth;
        public float countPerDamage = 1;
    }

    public class MutcompSpawnFilthOnHit : Mutcomp<MutcompSpawnFilthOnHitProps>
    {
        public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            int count = Mathf.RoundToInt(totalDamageDealt * props.countPerDamage);
            FilthMaker.TryMakeFilth(Pawn.Position, Pawn.Map, props.filth, Pawn.LabelIndefinite(), count, FilthSourceFlags.Natural);
        }
    }
}
