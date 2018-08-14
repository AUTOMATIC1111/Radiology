using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    public class Mutation :HediffWithComps
    {
        public new HediffMutationDef def => base.def as HediffMutationDef;
        
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);

            if (def.SpawnEffect(pawn) != null)
                def.SpawnEffect(pawn).Spawn(pawn.Map, pawn.TrueCenter());
        }

        public virtual IEnumerable<Gizmo> GetGizmos()
        {
            yield break;
        }
    }

    public class Mutation<T> : Mutation where T: HediffMutationDef
    {
        public new T def => base.def as T;
    }

}
