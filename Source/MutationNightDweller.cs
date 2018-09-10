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

    public class MutationNightDweller : MutationCapacityModifier<MutationNightDwellerDef>
    {
        public override float Multiplier()
        {
            float distanceFromMidday = Mathf.Abs(0.45f - GenLocalDate.DayPercent(pawn.Map));
            if (distanceFromMidday > 0.5f) distanceFromMidday = 1.0f - distanceFromMidday;
            float m = distanceFromMidday * 4 - 1f;
            return m < 0 ? m * def.negativeMultiplier : m * def.positiveMultiplier;
        }
    }
}
