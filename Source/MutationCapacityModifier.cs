using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Radiology
{
    public abstract class MutationCapacityModifier<T> :Mutation<T> where T :MutationDef
    {
        List<PawnCapacityModifier> modifiers;
        int lastTick = 0;

        public abstract float Multiplier();

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

                float m = Multiplier();

                for (int i = 0; i < modifiers.Count; i++)
                {
                    if (i >= stage.capMods.Count) break;
                    if (stage.capMods[i].capacity != modifiers[i].capacity)
                    {
                        Log.Error("Non-matching capacity for MutationCapacityModifier: [" + i + "] " + stage.capMods[i].capacity + " != " + modifiers[i].capacity);
                        break;
                    }

                    stage.capMods[i].offset = modifiers[i].offset * m;
                }

                pawn.health.capacities.Notify_CapacityLevelsDirty();

                return stage;
            }
        }
    }
}
