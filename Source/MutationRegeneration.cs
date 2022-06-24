using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    public class MutationRegenerationDef : MutationDef
    {
        public MutationRegenerationDef() { hediffClass = typeof(MutationRegeneration); }

        public int periodTicks = 600;
        public bool healMissingParts = false;

        public RadiologyEffectSpawnerDef effectRegeneration;
    }

    public class MutationRegeneration : Mutation<MutationRegenerationDef>
    {

        public bool RegenerateInjury()
        {
            var permanentInjuries = pawn.health.hediffSet.hediffs.OfType<Hediff_Injury>().Where(x => x.IsPermanent() && x.Part != null);
            Hediff_Injury injury = permanentInjuries.RandomElementWithFallback();
            if (injury == null) return false;

            HediffComp_GetsPermanent hediffComp_GetsPermanent = injury.TryGetComp<HediffComp_GetsPermanent>();
            if (hediffComp_GetsPermanent == null) return false;

            hediffComp_GetsPermanent.IsPermanent = false;
            injury.Severity = injury.Part.def.hitPoints - 1;
            pawn.health.hediffSet.DirtyCache();

            RadiologyEffectSpawnerDef.Spawn(def.effectRegeneration, pawn);
            return true;
        }

        public bool RegenerateBodyPart()
        {
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
            if (hediff == null) return false;

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

            RadiologyEffectSpawnerDef.Spawn(def.effectRegeneration, pawn);
            return true;
        }

        public override void Tick()
        {
            if (!pawn.IsHashIntervalTick(def.periodTicks)) return;

            if (RegenerateInjury()) return;

            if(def.healMissingParts) RegenerateBodyPart();
        }
    }
}
