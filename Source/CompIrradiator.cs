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
        Building linked;

        IEnumerable<ThingWithComps> GetFacilitiesBetweenThisAndTarget(Building target)
        {
            Rot4 rot = Rotation();

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

                if (rot.IsHorizontal && !MathHelper.IsBetween(thing.Position.x, parent.Position.x, target.Position.x)) continue;
                if (!rot.IsHorizontal && !MathHelper.IsBetween(thing.Position.z, parent.Position.z, target.Position.z)) continue;
                yield return thing;
            }

            yield break;
        }

        IEnumerable<T> GetModifiers<T, X>(Building target) where X : class where T : ThingComp
        {
            foreach (ThingWithComps thing in GetFacilitiesBetweenThisAndTarget(target))
            {
                foreach (T comp in thing.GetComps<T>())
                {
                    X x = comp as X;
                    if (x != null) yield return comp;
                }
            }

            yield break;
        }

        Rot4 Rotation()
        {
            if (linked == null) return Rot4.Invalid;

            IntVec3 a = parent.Position;
            IntVec3 b = linked.Position;

            int dx = b.x - a.x;
            int dz = b.z - a.z;

            if (Math.Abs(dx) >= Math.Abs(dz)) return dx > 0 ? Rot4.West : Rot4.East;
            else return dz > 0 ? Rot4.North : Rot4.South;
        }

        public virtual void CreateRadiation(RadiationInfo info, int ticks)
        {
            info.burn = props.burn.perSecond.RandomInRange;
            info.normal = props.mutate.perSecond.RandomInRange;
            info.rare = props.mutateRare.perSecond.RandomInRange;
        }

        public void Irradiate(Building target, RadiationInfo info, int ticks)
        {
            if (info.visited.Contains(this)) return;
            info.visited.Add(this);

            linked = target;
            if (linked == null) return;

            CreateRadiation(info, ticks);
            if (info.Empty()) return;

            ticksCooldown = ticks;
            bool playSound = props.soundIrradiate != null && info.visited.Count(x => x.props.soundIrradiate != null) < 2;
            soundSustainer = playSound ? props.soundIrradiate.TrySpawnSustainer(SoundInfo.InMap(parent, MaintenanceType.PerTick)) : null;

            motesReflectAt.Clear();
            foreach (ThingComp comp in GetModifiers<ThingComp, IRadiationModifier>(linked))
            {
                if (comp is CompBlocker)
                {
                    motesReflectAt.Add((Rotation().IsHorizontal ? comp.parent.Position.x : comp.parent.Position.z) + 0.5f);
                }

                if (info.secondHand) continue;

                IRadiationModifier modifier = comp as IRadiationModifier;
                modifier.Modify(ref info);
            }
        }

        public virtual string CanIrradiateNow(Pawn pawn)
        {
            if (powerComp == null || !powerComp.PowerOn)
                return "IrradiatorNoPower";

            return null;
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            powerComp = parent.TryGetComp<CompPowerTrader>();
        }

        public virtual CompPropertiesIrradiator MoteProps => props;

        public void SpawnRadiationMote()
        {
            if (linked == null) return;


            CompPropertiesIrradiator moteProps = MoteProps;
            if (moteProps?.motes == null || !moteProps.motes.Any()) return;

            var def = moteProps.motes.RandomElement();
            if (def.skip) return;

            if (def.speed.max < 0.5f)
            {
                if (Rand.RangeInclusive(0, 5) > linked.Position.DistanceTo(parent.Position)) return;
            }

            MoteRadiation moteThrown = ThingMaker.MakeThing(def.mote, null) as MoteRadiation;
            if (moteThrown == null) return;

            Vector3 origin = parent.ExactPosition();
            Vector3 destination = linked.ExactPosition();
            origin.y = destination.y;
            Vector3 sideways = (destination - origin).normalized.RotatedBy(90);

            origin += sideways * Rand.Range(-def.initialSpread, def.initialSpread);
            destination += sideways * Rand.Range(-def.spread, def.spread);

            float positionPercent = Mathf.Sqrt(Rand.Range(0f, 1f));
            float position = def.offset.LerpThroughRange(positionPercent);
            float scale = def.scaleRange.RandomInRange;

            Vector3 dir = destination - origin;
            Vector3 startOffset = dir * position;
            float angle = startOffset.AngleFlat();

            moteThrown.exactPosition = origin + startOffset;
            moteThrown.exactRotation = angle;
            moteThrown.reflectAt = motesReflectAt;
            moteThrown.reflectChance = def.reflectChance;

            Rot4 rot = Rotation();
            if (rot.IsHorizontal)
            {
                moteThrown.deathLocation = linked.ExactPosition().x;
                moteThrown.isHorizontal = true;
                moteThrown.reflectIndex = rot == Rot4.West ? 0 : motesReflectAt.Count() - 1;
                moteThrown.reflectIndexChange = rot == Rot4.West ? 1 : -1;
            }
            else
            {
                moteThrown.deathLocation = linked.ExactPosition().z;
                moteThrown.isHorizontal = false;
                moteThrown.reflectIndex = rot == Rot4.North ? 0 : motesReflectAt.Count() - 1;
                moteThrown.reflectIndexChange = rot == Rot4.North ? 1 : -1;
            }


            moteThrown.exactScale = new Vector3(scale, scale, scale);
            moteThrown.SetVelocity(angle, def.speed.RandomInRange);
            GenSpawn.Spawn(moteThrown, parent.Position, parent.Map, WipeMode.Vanish);
        }

        public override void CompTick()
        {
            ConsumePower();

            if (ticksCooldown <= 0) return;

            if (soundSustainer != null) soundSustainer.Maintain();

            SpawnRadiationMote();

            ticksCooldown--;
        }

        public void ConsumePower()
        {
            if (powerComp == null) return;

            if (ticksCooldown <= 0)
            {
                powerComp.PowerOutput = -powerComp.Props.basePowerConsumption;
            }
            else
            {
                powerComp.PowerOutput = -props.powerConsumption;
            }

        }

        Sustainer soundSustainer;

        public int ticksCooldown = 0;
        public List<float> motesReflectAt = new List<float>();

        private CompPowerTrader powerComp;
    }
}
