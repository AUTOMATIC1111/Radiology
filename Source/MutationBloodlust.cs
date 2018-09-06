using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Radiology
{
    public class MutationBloodlustDef : MutationDef
    {
        public MutationBloodlustDef() { hediffClass = typeof(MutationBloodlust); }
        
        public float aoe;
        public int maximumAmount = 1;
    }

    enum MyEnum { X, A, B, C };

    class MutationBloodlust : MutationCapacityModifier<MutationBloodlustDef>
    {
        public override float multiplier()
        {
            IntVec3 position = pawn.Position;

            int amount = pawn.Map.listerThings.ThingsOfDef(RimWorld.ThingDefOf.Filth_Blood)
                .Where(x => x.Position.DistanceTo(position) < def.aoe)
                .OfType<Filth>()
                .Select(x => x.thickness)
                .Sum();


            return 1.0f * Mathf.Min(amount, def.maximumAmount) / def.maximumAmount;
        }
    }
}
