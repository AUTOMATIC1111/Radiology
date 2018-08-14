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

        public float mtthDays;
        public float radius;
        public bool controlled;
        public bool aimed;

        public AutomaticEffectSpawnerDef effectOut;
        public AutomaticEffectSpawnerDef effectIn;
    }

    public class MutationBlink : Mutation<MutationBlinkDef>
    {
        public override void Tick()
        {
            base.Tick();

            if (!MathHelper.CheckMtthDays(def.mtthDays)) return;

            Blink();
        }


        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (!def.controlled) yield break;

            yield return new Command_Action
            {
                defaultLabel = "Blink!",
                defaultDesc = "Immediately telepor to random location",
                icon = ContentFinder<Texture2D>.Get("Radiology/Icons/BlinkIcon", true),
                action = delegate ()
                {
                    Blink();
                },
            };

            yield break;
        }
        private void Blink()
        {
            Vector3 position = pawn.Position.ToVector3();
            int attempts = 5;
            while (--attempts > 0)
            {
                Vector3 targetf = position + Vector3.right.RotatedBy(Rand.Range(0f, 360f)) * Rand.Range(1f, def.radius);
                IntVec3 target = targetf.ToIntVec3();

                if (!target.Walkable(pawn.Map)) continue;

                Blink(target);
                break;
            }
        }

        private void Blink(IntVec3 target)
        {
            if (def.effectOut != null) def.effectOut.Spawn(pawn.Map, pawn.TrueCenter());

            /// XXX add job interrupt and visual effects
            pawn.jobs.StartJob(new Job(RimWorld.JobDefOf.Wait, 1, false), JobCondition.InterruptForced, null, true, false);
            pawn.SetPositionDirect(target);
            pawn.Drawer.tweener.ResetTweenedPosToRoot();

            if (def.effectIn != null) def.effectIn.Spawn(pawn.Map, pawn.TrueCenter());

            //pawn.jobs.EndCurrentJob(Verse.AI.JobCondition.InterruptForced);
        }
    }
}