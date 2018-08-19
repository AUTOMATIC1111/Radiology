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
        public new MutationDef def => base.def as MutationDef;

        List<ThingComp> thingComps;

        void CreateThingComps()
        {
            if (thingComps != null) return;

            ThingComp[] comps = GetComps();
            if (comps == null) return;

            thingComps = new List<ThingComp>();
            List<ThingComp> list = pawn.Comps();

            foreach (ThingComp comp in comps)
            {
                comp.parent = pawn;
                thingComps.Add(comp);
                list.Add(comp);

                if (pawn.Map != null) comp.PostSpawnSetup(false);
            }
        }
        void RemoveThingComps()
        {
            if (thingComps == null) return;

            List<ThingComp> list = pawn.Comps();

            foreach (ThingComp comp in thingComps)
            {
                list.RemoveAll(x => x == comp);
            }

            thingComps = null;
        }

        void InitializeThingComps(bool remove)
        {
            ThingComp[] comps = GetComps();
            if (comps == null) return;

            List<ThingComp> list = pawn.Comps();
            foreach (ThingComp comp in comps)
            {
                if (remove)
                {
                    list.RemoveAll(x => x.GetType() == comp.GetType() && x.props == comp.props);
                    continue;
                }

                ThingComp existing = list.FirstOrDefault(x => x.GetType() == comp.GetType() && x.props == comp.props);
                if (existing != null) continue;

                comp.parent = pawn;
                list.Add(comp);

                if(pawn.Map != null) comp.PostSpawnSetup(false);
            }
        }

        void CreateApparel() => InitializeApparel(false);
        void RemoveApparel() => InitializeApparel(true);
        void InitializeApparel(bool remove)
        {
            if (def.apparel == null) return;

            List<Apparel> list = pawn.apparel.WornApparel;
            foreach (ThingDef apparelDef in def.apparel)
            {
                // this adds tools to the apparel so that damage capabilities are showin in the inventory tooltip
                foreach (HediffCompProperties_VerbGiver comp in def.comps.OfType<HediffCompProperties_VerbGiver>())
                {
                    foreach (Tool tool in comp.tools)
                    {
                        if (apparelDef.tools == null) apparelDef.tools = new List<Tool>();
                        if (!apparelDef.tools.Contains(tool))
                        {
                            apparelDef.tools.Add(tool);
                        }
                    }
                }

                Apparel existing = list.FirstOrDefault(x => x.def == apparelDef);
                
                if (remove)
                {
                    if (existing != null) pawn.apparel.Remove(existing);
                    existing.Destroy();
                    continue;
                }

                if (existing != null) continue;

                Apparel apparel = ThingMaker.MakeThing(apparelDef) as Apparel;
                if (apparel == null) continue;

                pawn.apparel.Wear(apparel);
            }
        }

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);

            RadiologyEffectSpawnerDef.Spawn(def.SpawnEffect(pawn), pawn);

            CreateThingComps();
            CreateApparel();
        }

        public override void PostRemoved()
        {
            RemoveThingComps();
            RemoveApparel();
        }

        public override void ExposeData()
        {
            base.ExposeData();
        }

        internal void PostLoad()
        {
            CreateThingComps();
            CreateApparel();
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

    public class Mutation<T> : Mutation where T : MutationDef
    {
        public new T def => base.def as T;
    }


}
