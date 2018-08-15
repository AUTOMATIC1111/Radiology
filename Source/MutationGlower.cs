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
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            parent.Map.mapDrawer.MapMeshDirty(parent.Position, MapMeshFlag.Things);
            parent.Map.glowGrid.RegisterGlower(this);
        }

        public override void PostDeSpawn(Map map)
        {
            parent.Map.mapDrawer.MapMeshDirty(parent.Position, MapMeshFlag.Things);
            parent.Map.glowGrid.DeRegisterGlower(this);
        }

        public override void CompTick()
        {
            base.CompTick();

            parent.Map.glowGrid.MarkGlowGridDirty(parent.Position);
        }
    }

    public class MutationGlowerDef : HediffMutationDef
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
