using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Radiology
{
    public class MutationSimulateIngestionDef : MutationDef
    {
        public MutationSimulateIngestionDef() { hediffClass = typeof(MutationSimulateIngestion); }

        public int periodTicks = 600;
        public float chance = 0.1f;
        public List<IngestionOutcomeDoer> outcomeDoers;
        public HediffDef stoppedByHediff;
    }
    public class MutationSimulateIngestion : Mutation<MutationSimulateIngestionDef>
    {
        public override void Tick()
        {
            if (Find.TickManager.TicksGame % def.periodTicks != 0) return;

            float chance = def.chance;
            if (def.stoppedByHediff != null)
            {
                Hediff stopper = pawn.health.hediffSet.GetFirstHediffOfDef(def.stoppedByHediff);
                if (stopper != null) chance *= Mathf.Max(0, 1f - stopper.Severity);
            }

            if (Rand.Value > chance) return;

            if (def.outcomeDoers != null)
            {
                for (int j = 0; j < def.outcomeDoers.Count; j++)
                {
                    def.outcomeDoers[j].DoIngestionOutcome(pawn, null);
                }
            }
        }
    }
}

