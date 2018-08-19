using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Radiology
{
    public class CompIrradiator : ThingComp
    {
        public new CompPropertiesIrradiator props => base.props as CompPropertiesIrradiator;

        public BodyPartRecord GetBodyPart(Chamber chamber, Pawn pawn)
        {
            if (pawn == null) return null;

            return pawn.health.hediffSet.GetNotMissingParts().RandomElementByWeight(x => chamber.def.GetPartWeight(pawn, x));
        }

        public HediffRadiation GetHediffRadition(BodyPartRecord part, Chamber chamber, Pawn pawn)
        {
            if (part == null) return null;

            foreach (var v in pawn.health.hediffSet.GetHediffs<HediffRadiation>())
            {
                if (v.Part == part) return v;
            }
            HediffRadiation hediff = HediffMaker.MakeHediff(HediffDefOf.RadiologyRadiation, pawn, part) as HediffRadiation;
            if (hediff == null) return hediff;

            pawn.health.AddHediff(hediff, null, null, null);
            return hediff;
        }

        IEnumerable<ThingWithComps> GetFacilitiesBetweenThisAndChamber(Chamber chamber)
        {
            Rot4 rot = parent.Rotation;

            IEnumerable<Thing> list =
                parent.GetComp<CompAffectedByFacilities>().LinkedFacilitiesListForReading
                .OrderBy(thing =>
                    rot == Rot4.North ? thing.Position.z :
                    rot == Rot4.South ? -thing.Position.z :
                    rot == Rot4.East ? thing.Position.x :
                    -thing.Position.x
                );

            foreach (var v in list)
            {
                ThingWithComps thing = v as ThingWithComps;
                if (thing == null) continue;

                if (parent.Rotation.IsHorizontal && !MathHelper.IsBetween(thing.Position.x, parent.Position.x, chamber.Position.x)) continue;
                if (!parent.Rotation.IsHorizontal && !MathHelper.IsBetween(thing.Position.z, parent.Position.z, chamber.Position.z)) continue;
                yield return thing;
            }

            yield break;
        }

        IEnumerable<T> GetModifiers<T, X>(Chamber chamber) where X: class where T : ThingComp
        {
            foreach (ThingWithComps thing in GetFacilitiesBetweenThisAndChamber(chamber))
            {
                foreach (T comp in thing.GetComps<T>())
                {
                    X x = comp as X;
                    if (x != null) yield return comp;
                }
            }

            yield break;
        }

        public void Irradiate(RadiationInfo info, int ticks)
        {
            Chamber chamber = parent.Linked<Chamber>();

            SoundDefOf.RadiologyIrradiateBasic.PlayOneShot(new TargetInfo(parent.Position, parent.Map, false));
            ticksCooldown = ticks;

            if (info.pawn.IsShielded()) return;

            info.part = GetBodyPart(info.chamber, info.pawn);
            info.burn = props.burn.perSecond.RandomInRange;
            info.normal = props.mutate.perSecond.RandomInRange;
            info.rare = props.mutateRare.perSecond.RandomInRange;
            if (info.secondHand) info.rare /= 2;

            motesReflectAt.Clear();
            foreach (ThingComp comp in GetModifiers<ThingComp, IRadiationModifier>(info.chamber))
            {
                if (comp is CompBlocker)
                {
                    motesReflectAt.Add((parent.Rotation.IsHorizontal ? comp.parent.Position.x : comp.parent.Position.z) + 0.5f);
                }

                if (info.secondHand) continue;

                IRadiationModifier modifier = comp as IRadiationModifier;
                modifier.Modify(ref info);
            }
            
            if (info.burn <= 0 && info.normal <= 0 && info.rare <= 0)
                return;

            HediffRadiation radiation = GetHediffRadition(info.part, info.chamber, info.pawn);
            if (radiation == null) return;

            radiation.burn += info.burn;
            radiation.normal += info.normal;
            radiation.rare += info.rare;

            float burnThreshold = info.chamber.def.burnThreshold.RandomInRange;
            float burnAmount = radiation.burn - burnThreshold;
            if (burnAmount > 0)
            {
                radiation.burn -= info.chamber.def.burnThreshold.min;

                DamageInfo dinfo = new DamageInfo(DamageDefOf.Burn, burnAmount * props.burn.multiplier, 999999f, -1f, info.chamber, radiation.Part, null, DamageInfo.SourceCategory.ThingOrUnknown, null);
                info.pawn.TakeDamage(dinfo);

                if (chamber != null)
                    RadiologyEffectSpawnerDef.Spawn(chamber.def.burnEffect, info.pawn);
            }

            float mutateThreshold = info.chamber.def.mutateThreshold.RandomInRange;
            float mutateAmount = radiation.normal + radiation.rare - mutateThreshold;
            if (mutateAmount > 0)
            {
                float ratio = radiation.rare / (radiation.normal + radiation.rare);
                radiation.rare -= info.chamber.def.mutateThreshold.min * ratio;
                radiation.normal -= info.chamber.def.mutateThreshold.min * (1f - ratio);

                Mutation mutation;
                var mutatedParts = RadiationHelper.MutatePawn(info.pawn, radiation, mutateAmount * props.mutate.multiplier, ratio, out mutation);
                if (mutatedParts != null)
                {
                    foreach (var anotherRadiation in info.pawn.health.hediffSet.GetHediffs<HediffRadiation>())
                    {
                        if (mutatedParts.Contains(anotherRadiation.Part) && radiation != anotherRadiation)
                        {
                            anotherRadiation.normal -= info.chamber.def.mutateThreshold.min * (1f - ratio);
                            anotherRadiation.rare -= info.chamber.def.mutateThreshold.min * ratio;
                        }
                    }
                }
            }
        }

        public static string causeNoPower = "IrradiatorNoPower";

        public string CanIrradiateNow(Pawn pawn)
        {
            if (powerComp == null || !powerComp.PowerOn)
                return causeNoPower;

            return null;
        }

        public bool IsHealthyEnoughForIrradiation(Chamber chamber, Pawn pawn)
        {
            var pawnParts = pawn.health.hediffSet.GetNotMissingParts();
            var parts = chamber.def.bodyParts.Join(pawnParts, left => left.part, right => right.def, (left, right) => right);

            foreach (var part in parts)
            {
                float health = PawnCapacityUtility.CalculatePartEfficiency(pawn.health.hediffSet, part, false, null);
                if (health < damageThreshold) return false;
            }

            return true;
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref damageThreshold, "damageThreshold");
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            powerComp = parent.TryGetComp<CompPowerTrader>();
        }

        List<Thing> Chambers => parent.GetComp<CompFacility>().LinkedBuildings();

        public void SpawnRadiationMote()
        {
            if (props.motes == null || !props.motes.Any()) return;
            var def = props.motes.RandomElement();
            if (def.skip) return;

            MoteRadiation moteThrown = ThingMaker.MakeThing(def.mote, null) as MoteRadiation;
            if (moteThrown == null) return;

            List<Thing> chambers = Chambers;
            if (!chambers.Any()) return;
            Thing chamber = chambers.RandomElement();

            Vector2 rotationVector2 = parent.Rotation.AsVector2.RotatedBy(90);
            Vector3 rotationVector = new Vector3(rotationVector2.x, 0, rotationVector2.y);
            Vector3 origin = parent.ExactPosition() + Rand.Range(-def.initialSpread, def.initialSpread) * rotationVector;
            Vector3 destination = chamber.ExactPosition() + Rand.Range(-def.spread, def.spread) * rotationVector;
            Vector3 offset = destination - origin;
            Vector3 dir = offset.normalized;

            float positionPercent = Mathf.Sqrt(Rand.Range(0f,1f));
            float position = def.offset.LerpThroughRange(positionPercent);
            float scale = def.scaleRange.RandomInRange;

            Vector3 startOffset = offset * position;
            float angle = startOffset.AngleFlat();

            moteThrown.exactPosition = origin + startOffset;
            moteThrown.exactRotation = angle;
            moteThrown.reflectAt = motesReflectAt;
            moteThrown.reflectChance = def.reflectChance;

            if (parent.Rotation.IsHorizontal)
            {
                moteThrown.deathLocation = chamber.Position.x + 0.5f;
                moteThrown.isHorizontal = true;
                moteThrown.reflectIndex = parent.Rotation == Rot4.West ? 0 : motesReflectAt.Count() - 1;
                moteThrown.reflectIndexChange = parent.Rotation == Rot4.West ? 1 : -1;
            }
            else
            {
                moteThrown.deathLocation = chamber.Position.z + 0.5f;
                moteThrown.isHorizontal = false;
                moteThrown.reflectIndex = parent.Rotation == Rot4.North ? 0 : motesReflectAt.Count() - 1;
                moteThrown.reflectIndexChange = parent.Rotation == Rot4.North ? 1 : -1;
            }

 
            moteThrown.exactScale = new Vector3(scale, scale, scale);
            moteThrown.SetVelocity(angle, def.speed.RandomInRange);
            GenSpawn.Spawn(moteThrown, parent.Position, parent.Map, WipeMode.Vanish);
        }

        public override void CompTick()
        {
            if (powerComp == null) return;

            if (ticksCooldown <= 0)
            {
                powerComp.PowerOutput = -powerComp.Props.basePowerConsumption;
                return;
            }

            powerComp.PowerOutput = -props.powerConsumption;

            SpawnRadiationMote();

            ticksCooldown--;
        }

        public float damageThreshold = 0.5f;
        public int ticksCooldown=0;
        public List<float> motesReflectAt = new List<float>();

        private CompPowerTrader powerComp;
    }
}
