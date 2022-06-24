using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace Radiology
{
    class JobDriverIrradiatePrisoner : JobDriver
    {
        private Chamber Chamber => job.GetTarget(TargetIndex.B).Thing as Chamber;
        private Pawn Prisoner => job.GetTarget(TargetIndex.A).Thing as Pawn;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (Chamber == null) return false;

            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed) && pawn.Reserve(job.targetB, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetIndex.A);
            this.FailOnBurningImmobile(TargetIndex.B);
            this.FailOnDowned(TargetIndex.A);
            this.FailOnAggroMentalState(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOn(() => !Prisoner.IsPrisonerOfColony || !Prisoner.guest.PrisonerIsSecure).FailOnSomeonePhysicallyInteracting(TargetIndex.A);
            yield return Toils_Haul.StartCarryThing(TargetIndex.A, false, false, false);
            Toil carryToCell = Toils_Haul.CarryHauledThingToCell(TargetIndex.B);
            yield return carryToCell;
            yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.B, carryToCell, false);
            Toil setupIrradiation = new Toil()
            {
                initAction = delegate ()
                {
                    pawn.ClearReservationsForJob(job);

                    Job prisonerJob = new Job(JobDefOf.RadiologyIrradiate, Chamber, 1500, true);
                    Prisoner.jobs.StartJob(prisonerJob);
                }
            };
            yield return setupIrradiation;
            yield break;
        }
    }
}
