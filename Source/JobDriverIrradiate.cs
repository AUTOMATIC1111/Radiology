using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace Radiology
{
    class JobDriverIrradiate :JobDriver, IPrisonerAllowedJob
    {
        private Chamber Chamber => job.GetTarget(TargetIndex.A).Thing as Chamber;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (Chamber == null) return false;

            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOn(delegate () { return Chamber.CanIrradiateNow(pawn) != null; });
 //           AddFinishAction(delegate () { if (pawn.IsPrisoner) pawn.SetForbidden(false); });

            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell).FailOnDespawnedOrNull(TargetIndex.A);
            Toil work = new Toil();
            work.defaultCompleteMode = ToilCompleteMode.Never;
            work.WithEffect(EffecterDefOf.Drill, TargetIndex.A);
            work.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);

            work.tickAction = delegate ()
            {
                if (pawn.IsHashIntervalTick(60))
                    Chamber.Irradiate(work.actor, 60);
            };
            yield return work;
            yield break;
        }
    }
}
