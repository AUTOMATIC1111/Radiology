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

        public float range;
        public float drain;
        public float capacity;
        public float efficiency = 1.0f;
        public float discargePerSecond;

        public RadiologyEffectSpawnerDef effectDraining;
        public RadiologyEffectSpawnerDef effectDrained;
    }

    public class MutationBattery : MutationCapacityModifier<MutationBatteryDef>
    {
        Building previousBuilding = null;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref charge, "charge");
        }

        public override float Multiplier()
        {
            Drain();

            float res = charge / def.capacity * 2 - 1f;

            charge -= def.discargePerSecond;
            if (charge < 0) charge = 0;

            return res;
        }

        Vector3 halfNeg = new Vector3(-0.5f, 0f, -0.5f);
        public void Drain()
        {
            Building building = null;
            if (previousBuilding != null && previousBuilding.Position.DistanceTo(pawn.Position) < def.range) building = previousBuilding;
            if (building == null) building = pawn.Map.listerBuildings.allBuildingsColonistElecFire.FirstOrDefault(x => x.Position.DistanceTo(pawn.Position) < def.range);
            previousBuilding = building;

            if (building == null) return;

            CompPower power = building.GetComp<CompPower>();
            if (power == null || power.PowerNet == null) return;

            float amount = Mathf.Min(power.PowerNet.AvailablePower(), def.drain);
            float canTake = def.efficiency < 0.0001 ? def.capacity : (def.capacity - charge) / def.efficiency;

            if (amount > canTake) amount = canTake;
            if (amount < def.drain / 2) return;

            power.PowerNet.Drain(amount);
            charge += amount * def.efficiency;

            RadiologyEffectSpawnerDef.Spawn(def.effectDraining, pawn.Map, pawn.TrueCenter(), (building.Position - pawn.Position).AngleFlat);
            RadiologyEffectSpawnerDef.Spawn(def.effectDrained, pawn.Map, building.RandomPointNearTrueCenter(), (pawn.Position - building.Position).AngleFlat);
        }

        float charge = 0.0f;
    }
}
