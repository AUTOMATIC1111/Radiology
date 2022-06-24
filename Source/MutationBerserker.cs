using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiology
{
    public class MutationBerserkerDef : MutationDef
    {
        public MutationBerserkerDef() { hediffClass = typeof(MutationBerserker); }
    }

    class MutationBerserker : MutationCapacityModifier<MutationBerserkerDef>
    {
        public override float Multiplier()
        {
            return 1f - pawn.health.summaryHealth.SummaryHealthPercent;
        }
    }
}
