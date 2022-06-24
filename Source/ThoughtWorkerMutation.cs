using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    class ThoughtWorkerMutationUgly : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
        {
            if (!p.RaceProps.Humanlike || !RelationsUtility.PawnsKnowEachOther(p, other) || other.def != p.def) return false;

            var mutations = other.health.hediffSet.GetHediffs<Mutation>().Where(x => x.def.beauty < 0);

            int beauty = mutations.Sum(x => x.def.beauty);
            int impact = beauty / 5;
            if (impact >= 0) return false;

            string reason = string.Join(", ", mutations.Select(x => x.def.LabelCap).ToArray());

            return ThoughtState.ActiveAtStage(-impact - 1, reason);
        }
    }
    class ThoughtWorkerMutationBeautiful: ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
        {
            if (!p.RaceProps.Humanlike || !RelationsUtility.PawnsKnowEachOther(p, other) || other.def != p.def) return false;

            var mutations = other.health.hediffSet.GetHediffs<Mutation>().Where(x => x.def.beauty > 0);

            int beauty = mutations.Sum(x => x.def.beauty);
            int impact = beauty / 5;
            if (impact <= 0) return false;

            string reason = string.Join(", ", mutations.Select(x => x.def.LabelCap).ToArray());

            return ThoughtState.ActiveAtStage(impact - 1, reason);
        }
    }
}
