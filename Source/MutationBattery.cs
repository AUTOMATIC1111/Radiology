using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Radiology
{
    public class MutationBatteryDef : MutationDef
    {
        public MutationBatteryDef() { hediffClass = typeof(MutationBattery); }

        public int periodTicks;
        public float range;
        public float drain;

        public RadiologyEffectSpawnerDef effectDraining;
        public RadiologyEffectSpawnerDef effectDrained;
    }

    public class MutationBattery : Mutation<MutationBatteryDef>
    {
        Building previousBuilding = null;


        Vector3 halfNeg = new Vector3(-0.5f, 0f, -0.5f);
        public override void Tick()
        {
            base.Tick();

            if (!pawn.IsHashIntervalTick(def.periodTicks)) return;

            Building building = null;
            if (previousBuilding != null && previousBuilding.Position.DistanceTo(pawn.Position) < def.range) building = previousBuilding;
            if (building == null) building = pawn.Map.listerBuildings.allBuildingsColonistElecFire.FirstOrDefault(x => x.Position.DistanceTo(pawn.Position) < def.range);
            previousBuilding = building;

            if (building == null) return;

            CompPower power = building.GetComp<CompPower>();
            if (power == null || power.PowerNet==null) return;

            float amount = Mathf.Min(power.PowerNet.AvailablePower(), def.drain);
            if (amount < def.drain/2) return;

            power.PowerNet.Drain(amount);

            RadiologyEffectSpawnerDef.Spawn(def.effectDraining, pawn.Map, pawn.TrueCenter(), (building.Position - pawn.Position).AngleFlat);
            RadiologyEffectSpawnerDef.Spawn(def.effectDrained, pawn.Map, building.RandomPointNearTrueCenter(), (pawn.Position - building.Position).AngleFlat);
        }
    }
}
