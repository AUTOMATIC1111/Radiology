using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    public class RecipeRedoDiagnosis : Recipe_Surgery
    {
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            foreach (Cancer cancer in pawn.health.hediffSet.GetHediffs<Cancer>())
            {
                if (! cancer.diagnosed) continue;
                if (cancer.Part == null) continue;

                yield return cancer.Part;
            }
            
            yield break;
        }

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            Hediff hediff = pawn.health.hediffSet.hediffs.Find(x => x is Cancer && x.Part == part && x.Visible);
            Cancer cancer = hediff as Cancer;
            if (cancer == null) return;

            Thing medicine = ingredients.FirstOrDefault();
            float quality = TendUtility.CalculateBaseTendQuality(billDoer, pawn, (medicine == null) ? null : medicine.def);

            cancer.Tended(quality, 0);
        }
    }
}
