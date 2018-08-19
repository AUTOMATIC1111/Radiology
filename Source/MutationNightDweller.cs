using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Radiology
{
    public class MutationNightDwellerDef : MutationDef
    {
        public MutationNightDwellerDef() { hediffClass = typeof(MutationNightDweller); }

        public float positiveMultiplier;
        public float negativeMultiplier;
    }

    public class MutationNightDweller : Mutation<MutationNightDwellerDef>
    {
        List<PawnCapacityModifier> modifiers;
        int lastTick = 0;

        public override HediffStage CurStage
        {
            get
            {
                if (base.CurStage == null) return null;
                HediffStage stage = def.stages[0];

                if (Find.TickManager.TicksGame < lastTick + 60 || pawn.Map == null) return stage;
                lastTick = Find.TickManager.TicksGame;

                if (modifiers == null)
                {
                    modifiers = new List<PawnCapacityModifier>();
                    foreach (var v in stage.capMods)
                    {
                        modifiers.Add(new PawnCapacityModifier() { capacity = v.capacity, offset = v.offset });
                    }
                }

                float distanceFromMidday = Mathf.Abs(0.45f - GenLocalDate.DayPercent(pawn.Map));
                if (distanceFromMidday > 0.5f) distanceFromMidday = 1.0f - distanceFromMidday;
                float multiplier = distanceFromMidday * 4 - 1f;

                for (int i = 0; i < modifiers.Count; i++)
                {
                    if (i >= stage.capMods.Count) break;
                    if (stage.capMods[i].capacity != modifiers[i].capacity)
                    {
                        Log.Error("Non-matching capacity for MutationNightDweller: ["+i+"] "+ stage.capMods[i].capacity+" != "+ modifiers[i].capacity);
                        break;
                    }

                    float offset = modifiers[i].offset * multiplier;
                    offset *= offset < 0 ? def.negativeMultiplier : def.positiveMultiplier;
                    stage.capMods[i].offset = offset;
                }

                pawn.health.capacities.Notify_CapacityLevelsDirty();

                return stage;
            }
        }
    }
}
