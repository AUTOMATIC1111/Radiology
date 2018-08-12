using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    public abstract class CompFilterOperationMode : IRadiationModifier
    {
        public string Label;
        public ResearchProjectDef Prerequisite;

        public virtual void Modify(ref RadiationInfo info)
        {
            throw new NotImplementedException();
        }

        public virtual bool Available => Prerequisite == null ? true : Prerequisite.IsFinished;
    }

    public class CompFilterOperationModeBasic : CompFilterOperationMode
    {
        public CompFilterOperationModeBasic()
        {
            Label = "filter out 20% of burn radiation and 10% of others";
        }

        public override void Modify(ref RadiationInfo info)
        {
            info.burn *= 0.8f;
            info.normal *= 0.9f;
            info.rare *= 0.9f;
        }
    }

    public class CompFilterOperationModeIntermediate : CompFilterOperationMode
    {
        public CompFilterOperationModeIntermediate()
        {
            Label = "filter out 30% of burn radiation and 15% of others";
            Prerequisite = ResearchDefOf.RadiologyFilteringIntermediate;
        }

        public override void Modify(ref RadiationInfo info)
        {
            info.burn *= 0.7f;
            info.normal *= 0.85f;
            info.rare *= 0.85f;
        }
    }

    public class CompFilterOperationModeAdvanced : CompFilterOperationMode
    {
        public CompFilterOperationModeAdvanced()
        {
            Label = "filter out 40% of burn radiation and 20% of others";
            Prerequisite = ResearchDefOf.RadiologyFilteringAdvanced;
        }

        public override void Modify(ref RadiationInfo info)
        {
            info.burn *= 0.6f;
            info.normal *= 0.8f;
            info.rare *= 0.8f;
        }
    }

    public class CompFilterOperationModeRemoveNormal : CompFilterOperationMode
    {
        public CompFilterOperationModeRemoveNormal()
        {
            Label = "filter out 20% of normal radiation";
        }

        public override void Modify(ref RadiationInfo info)
        {
            info.normal *= 0.8f;
        }
    }

    public class CompFilterOperationModeRemoveNormalIntermediate : CompFilterOperationMode
    {
        public CompFilterOperationModeRemoveNormalIntermediate()
        {
            Label = "filter out 30% of normal radiation";
            Prerequisite = ResearchDefOf.RadiologyFilteringIntermediate;
        }

        public override void Modify(ref RadiationInfo info)
        {
            info.normal *= 0.7f;
        }
    }

    public class CompFilterOperationModeRemoveNormalAdvanced : CompFilterOperationMode
    {
        public CompFilterOperationModeRemoveNormalAdvanced()
        {
            Label = "filter out 40% of normal radiation";
            Prerequisite = ResearchDefOf.RadiologyFilteringAdvanced;
        }

        public override void Modify(ref RadiationInfo info)
        {
            info.normal *= 0.6f;
        }
    }

    public class CompFilterOperationModeTradeoffRemoveBurn : CompFilterOperationMode
    {
        public float multiplier = 5f;

        public CompFilterOperationModeTradeoffRemoveBurn()
        {
            Label = "use rare radiation to filter out 5x burn radiation";
            Prerequisite = ResearchDefOf.RadiologyFilteringTradeoff;
        }

        public override void Modify(ref RadiationInfo info)
        {
            if (info.rare * multiplier < info.burn)
            {
                info.burn -= info.rare * multiplier;
                info.rare = 0;
            }
            else
            {
                info.burn = 0;
                info.rare -= info.burn / multiplier;
            }
        }
    }

    public class CompFilterOperationModeTradeoffRemoveNormal : CompFilterOperationMode
    {
        public float multiplier = 5f;

        public CompFilterOperationModeTradeoffRemoveNormal()
        {
            Label = "use rare radiation to filter out 5x normal radiation";
            Prerequisite = ResearchDefOf.RadiologyFilteringTradeoff;
        }

        public override void Modify(ref RadiationInfo info)
        {
            if (info.rare * multiplier < info.normal)
            {
                info.normal -= info.rare * multiplier;
                info.rare = 0;
            }
            else
            {
                info.normal = 0;
                info.rare -= info.normal / multiplier;
            }
        }
    }

    public class CompFilterOperationModeDouble : CompFilterOperationMode
    {
        public CompFilterOperationModeDouble()
        {
            Label = "double all radiation";
            Prerequisite = ResearchDefOf.RadiologyFilteringAdvanced;
        }

        public override void Modify(ref RadiationInfo info)
        {
            info.burn *= 2;
            info.normal *= 2;
            info.rare *= 2;
        }
    }
}
