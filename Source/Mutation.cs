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

        public void InitializeThingComps()
        {
            ThingComp[] comps = GetComps();
            if (comps == null) return;

            foreach (ThingComp comp in comps)
            {
                ThingComp existing = pawn.Comps().FirstOrDefault(x =>
                    x.GetType() == comp.GetType() && x.props == comp.props
                );
                
                if (existing != null) continue;

                comp.parent = pawn;
                pawn.Comps().Add(comp);

                if(pawn.Map != null) comp.PostSpawnSetup(false);
            }
        }

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);

            AutomaticEffectSpawnerDef.Spawn(def.SpawnEffect(pawn), pawn);

            InitializeThingComps();
        }

        public override void ExposeData()
        {
            base.ExposeData();
        }

        internal void PostLoad()
        {
            InitializeThingComps();
        }

        public virtual IEnumerable<Gizmo> GetGizmos()
        {
            yield break;
        }

        public virtual ThingComp[] GetComps()
        {
            return null;
        }
    }
    
    public class Mutation<T> : Mutation where T : HediffMutationDef
    {
        public new T def => base.def as T;
    }
}
