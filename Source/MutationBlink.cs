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

    }

    public class MutationBlink : Mutation<MutationBlinkDef>
    {
        public override void Tick()
        {
            base.Tick();

            if (!MathHelper.CheckMtthDays(def.mtthDays)) return;

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
            /// XXX add job interrupt and visual effects
            pawn.SetPositionDirect(target);
            pawn.jobs.StartJob(new Job(RimWorld.JobDefOf.Wait, 1, false), JobCondition.InterruptForced, null, true, false);

            //pawn.jobs.EndCurrentJob(Verse.AI.JobCondition.InterruptForced);
        }
    }
}