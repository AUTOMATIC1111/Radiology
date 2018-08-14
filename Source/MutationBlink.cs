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
    public class MutationBlinkDef : HediffMutationDef
    {
        public MutationBlinkDef() { hediffClass = typeof(MutationBlink); }

        public float mtthDays=-1;
        public float radius;
        public bool controlled;
        public bool aimed;
        public int cooldownTicks = 240;

        public AutomaticEffectSpawnerDef effectOut;
        public AutomaticEffectSpawnerDef effectIn;
    }

    public class MutationBlink : Mutation<MutationBlinkDef>
    {
        public override void Tick()
        {
            base.Tick();

            if (cooldown > 0) cooldown--;

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
            if (target.DistanceTo(pawn.Position) <= def.radius)
            {
                Blink(target);
                return;
            }

            Vector3 pos = pawn.Position.ToVector3();
            pos = pos + (target.ToVector3() - pos).normalized * def.radius;
            IntVec3 newTarget = pos.ToIntVec3();
            if (newTarget.Walkable(pawn.Map))
            {
                Blink(newTarget);
                return;
            }

            BlinkRandomly(pos, 8);
        }

        private void Blink(IntVec3 target)
        {
            cooldown = def.cooldownTicks;

            if (def.effectOut != null) def.effectOut.Spawn(pawn.Map, pawn.TrueCenter());

            pawn.jobs.StartJob(new Job(RimWorld.JobDefOf.Wait, 1, false), JobCondition.InterruptForced, null, true, false);
            pawn.SetPositionDirect(target);
            pawn.Drawer.tweener.ResetTweenedPosToRoot();

            if (def.effectIn != null) def.effectIn.Spawn(pawn.Map, pawn.TrueCenter());
        }

        public int cooldown = 0;
    }
}