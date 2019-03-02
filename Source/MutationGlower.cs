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

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            parent.Map.mapDrawer.MapMeshDirty(parent.Position, MapMeshFlag.Things);
            parent.Map.glowGrid.RegisterGlower(this);
        }

        public override void PostDeSpawn(Map map)
        {
            map.glowGrid.DeRegisterGlower(this);
            if (parent != null) map.mapDrawer.MapMeshDirty(parent.Position, MapMeshFlag.Things);
            map.glowGrid.MarkGlowGridDirty(parent.Position);
        }

        public override void CompTick()
        {
            base.CompTick();
            
            if (previousPosition == parent.Position) return;

            previousPosition = parent.Position;
            if (parent.Map != null)  parent.Map.glowGrid.MarkGlowGridDirty(parent.Position);
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
