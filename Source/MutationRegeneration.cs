using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    public class CompRegeneration : ThingComp
    {
        public MutationRegeneration mutation;

        public override void CompTick()
        {
            mutation.RegenerateTick();
        }
    }
    public class MutationRegenerationDef : MutationDef
    {
        public MutationRegenerationDef() { hediffClass = typeof(MutationRegeneration); }

        public RadiologyEffectSpawnerDef effectRegeration;
    }

    public class MutationRegeneration : Mutation<MutationRegenerationDef>
    {
        public override ThingComp[] GetComps()
        {
            return new ThingComp[] { new CompRegeneration() { mutation = this } };
        }

        public void RegenerateTick()
        {
            base.Tick();

            if (!pawn.IsHashIntervalTick(600)) return;

            var injured = pawn.health.hediffSet.GetInjuredParts();
            var missing = pawn.health.hediffSet.GetMissingPartsCommonAncestors();
            var potential = missing.Where(x =>
                x.Part != null && (
                    x.Part.parent == null || (
                        !injured.Contains(x.Part.parent) &&
                        !injured.Intersect(x.Part.parent.GetDirectChildParts()).Any()
                    )
                )
            );

            Hediff_MissingPart hediff = potential.RandomElementWithFallback();
            if (hediff == null) return;

            BodyPartRecord part = hediff.Part;
            pawn.health.hediffSet.hediffs.Remove(hediff);

            foreach (var subpart in part.GetDirectChildParts())
            {
                Hediff_MissingPart missingPart = HediffMaker.MakeHediff(RimWorld.HediffDefOf.MissingBodyPart, pawn, subpart) as Hediff_MissingPart;
                pawn.health.hediffSet.hediffs.Add(missingPart);
            }
            
            Hediff_Injury injury = HediffMaker.MakeHediff(RimWorld.HediffDefOf.Shredded, pawn, part) as Hediff_Injury;
            injury.Severity = part.def.hitPoints - 1;
            pawn.health.hediffSet.hediffs.Add(injury);

            pawn.health.hediffSet.DirtyCache();

            RadiologyEffectSpawnerDef.Spawn(def.effectRegeration, pawn);
        }
    }
}
