using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    public class CompOrganicGlower : CompGlower
    {
        IntVec3 previousPosition;
        Map previousMap = null;
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {

        }

        public override void PostDeSpawn(Map map)
        {

        }

        public void Register(Map map)
        {
            map.mapDrawer.MapMeshDirty(parent.Position, MapMeshFlag.Things);
            map.glowGrid.RegisterGlower(this);
        }

        public void Unregister(Map map)
        {
            map.glowGrid.DeRegisterGlower(this);
            if (parent != null) map.mapDrawer.MapMeshDirty(parent.Position, MapMeshFlag.Things);
            map.glowGrid.MarkGlowGridDirty(parent.Position);
        }

        public override void CompTick()
        {
            base.CompTick();

            if (parent == null) return;
            Map map = parent.Map;

            if (previousMap != map)
            {
                if (previousMap != null) Unregister(previousMap);
                if (map != null) Register(map);

                previousMap = map;
            }

            if (map != null && previousPosition != parent.Position)
            {
                map.glowGrid.MarkGlowGridDirty(parent.Position);

                previousPosition = parent.Position;
            }
        }
    }

    public class MutationGlowerDef : MutationDef
    {
        public MutationGlowerDef() { hediffClass = typeof(MutationGlower); }

        public CompProperties_Glower glow;
    }

    public class MutationGlower : Mutation<MutationGlowerDef>
    {
        public override ThingComp[] GetComps()
        {
            return new ThingComp[] { new CompOrganicGlower() { props = def.glow } };
        }
    }
}
