using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Radiology
{
    public class MutationBlinkDef : MutationDef
    {
        public MutationBlinkDef() { hediffClass = typeof(MutationBlink); }

        public float mtthDays=-1;
        public float radius;
        public bool controlled;
        public bool aimed;
        public int cooldownTicks = 240;

        public RadiologyEffectSpawnerDef effectOut;
        public RadiologyEffectSpawnerDef effectIn;
    }

    public class MutationBlink : Mutation<MutationBlinkDef>
    {
        public override void Tick()
        {
            base.Tick();

            if (cooldown > 0) cooldown--;
            if (pawn.Map == null) return;
            if (!MathHelper.CheckMtthDays(def.mtthDays)) return;

            BlinkRandomly();
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref cooldown, "cooldown");
        }
        
        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (!def.controlled) yield break;

            if (def.aimed)
            {
                yield return new Command_Target_Location
                {
                    defaultLabel = "RadiologyMutationBlinkCommand".Translate(),
                    defaultDesc = "RadiologyMutationBlinkTargetedDesc".Translate(),
                    icon = ContentFinder<Texture2D>.Get("Radiology/Icons/BlinkIcon", true),
                    action = delegate (LocalTargetInfo target)
                    {
                        BlinkTargeted(target.Cell);
                    },
                    targetingParams = new TargetingParameters()
                    {
                        canTargetLocations = true,
                    },
                    disabled = cooldown > 0,
                };
            }
            else
            {
                yield return new Command_Action
                {
                    defaultLabel = "RadiologyMutationBlinkCommand".Translate(),
                    defaultDesc = "RadiologyMutationBlinkDesc".Translate(),
                    icon = ContentFinder<Texture2D>.Get("Radiology/Icons/BlinkIcon", true),
                    action = delegate ()
                    {
                        BlinkRandomly();
                    },
                    disabled = cooldown > 0,
                };
            }


            yield break;
        }

        private void BlinkRandomly()
        {
            BlinkRandomly(pawn.Position.ToVector3(), def.radius);
        }

        private void BlinkRandomly(Vector3 position, float radius)
        {
            int attempts = 5;
            while (--attempts > 0)
            {
                Vector3 targetf = position + Vector3.right.RotatedBy(Rand.Range(0f, 360f)) * Rand.Range(1f, radius);
                IntVec3 target = targetf.ToIntVec3();

                if (!target.Walkable(pawn.Map)) continue;

                Blink(target);
                break;
            }
        }

        private void BlinkTargeted(IntVec3 target)
        {
            float distanceToTarget = target.DistanceTo(pawn.Position);
            if (distanceToTarget <= def.radius && !target.Impassable(pawn.Map))
            {
                Blink(target);
                return;
            }

            Vector3 pos = pawn.Position.ToVector3();
            Vector3 dest = target.ToVector3();
            Vector3 dir = (dest - pos).normalized;
            pos = pos + dir * Mathf.Min(distanceToTarget, def.radius);
            for (float distance = distanceToTarget; distance >= 0; distance -= 1)
            {
                IntVec3 newTarget = pos.ToIntVec3();
                if (! newTarget.Impassable(pawn.Map))
                {
                    Blink(newTarget);
                    return;
                }
                pos -= dir; 
            }

            Blink(pawn.Position);
        }

        private void Blink(IntVec3 target)
        {
            cooldown = def.cooldownTicks;

            RadiologyEffectSpawnerDef.Spawn(def.effectOut, pawn);

            pawn.jobs.StartJob(new Job(RimWorld.JobDefOf.Wait, 1, false), JobCondition.InterruptForced, null, true, false);
            pawn.SetPositionDirect(target);
            pawn.Drawer.tweener.ResetTweenedPosToRoot();

            RadiologyEffectSpawnerDef.Spawn(def.effectIn, pawn);
        }

        public int cooldown = 0;
    }
}