using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    // reduced capacity
    public class CancerCompCapacityOffsetDef : CancerCompDef
    {
        public CancerCompCapacityOffsetDef()
        {
            Init(typeof(CancerCompCapacityOffset));
        }

        public float offset;
        public PawnCapacityDef capacity;
    }

    public class CancerCompCapacityOffset : CancerComp<CancerCompCapacityOffsetDef>
    {
        public override object[] DescriptionArgs => new object[] { def.capacity.label, (int) (-Offset * 100) };

        public float Offset => def.offset * cancer.Severity;

        public override void Update(int passed) => cancer.stage.capMods.Add( new PawnCapacityModifier() { capacity = def.capacity, offset = Offset } );
    }

    // reduced body part efficiency
    public class CancerCompPartEfficiencyDef : CancerCompDef
    {
        public CancerCompPartEfficiencyDef()
        {
            Init(typeof(CancerCompPartEfficiency));
        }

        public float offset;
    }

    public class CancerCompPartEfficiency : CancerComp<CancerCompPartEfficiencyDef>
    {
        public override object[] DescriptionArgs => new object[] { (int)(-Offset * 100) };

        public float Offset => def.offset * cancer.Severity;

        public override void Update(int passed) => cancer.stage.partEfficiencyOffset = Offset;
    }

    // growth/shrinking
    public class CancerCompSeverityChangeDef : CancerCompDef
    {
        public CancerCompSeverityChangeDef()
        {
            Init(typeof(CancerCompSeverityChange));
        }

        public float changePerDay;
    }

    public class CancerCompSeverityChange : CancerComp<CancerCompSeverityChangeDef>
    {
        public override void Update(int passed) => cancer.Severity += passed * def.changePerDay / 60000;
    }


    // destory body part at certain severity
    public class CancerCompDestroyBodyPartDef : CancerCompDef
    {
        public CancerCompDestroyBodyPartDef()
        {
            Init(typeof(CancerCompDestroyBodyPart));
        }

        public float destroyAtSeverity;
    }

    public class CancerCompDestroyBodyPart : CancerComp<CancerCompDestroyBodyPartDef>
    {
        public override object[] DescriptionArgs => new object[] { cancer.Part.Label, (int)(def.destroyAtSeverity * 100) };

        public override void Update(int passed) => cancer.stage.destroyPart = cancer.Severity >= def.destroyAtSeverity;
    }


    // increased hunger levels
    public class CancerCompHungerDef : CancerCompDef
    {
        public CancerCompHungerDef()
        {
            Init(typeof(CancerCompHunger));
        }

        public float offset;
    }

    public class CancerCompHunger : CancerComp<CancerCompHungerDef>
    {
        public override object[] DescriptionArgs => new object[] { (int)(cancer.Severity * def.offset * 100) };

        public override void Update(int passed) => cancer.stage.hungerRateFactorOffset = cancer.Severity * def.offset;
    }


    // pain
    public class CancerCompPainDef : CancerCompDef
    {
        public CancerCompPainDef() { Init(typeof(CancerCompPain)); }

        public float offset;
    }

    public class CancerCompPain : CancerComp<CancerCompPainDef>
    {
        public override void Update(int passed) => cancer.stage.painOffset = cancer.Severity * def.offset;

    }

    // mental break
    public class CancerCompMentalBreakDef : CancerCompDef
    {
        public CancerCompMentalBreakDef()
        {
            Init(typeof(CancerCompMentalBreak));
        }

        public float mtbDays;
    }

    public class CancerCompMentalBreak : CancerComp<CancerCompMentalBreakDef>
    {
        public override void Update(int passed) => cancer.stage.mentalBreakMtbDays = def.mtbDays;
    }


    // aggressiveness (social fight chance)
    public class CancerCompAggressivenessDef : CancerCompDef
    {
        public CancerCompAggressivenessDef()
        {
            Init(typeof(CancerCompAggressiveness));
        }

        public float offset;
    }

    public class CancerCompAggressiveness : CancerComp<CancerCompAggressivenessDef>
    {
        public override void Update(int passed) => cancer.stage.socialFightChanceFactor = cancer.Severity * def.offset;
    }

    // vomiting
    public class CancerCompVomitingDef : CancerCompDef
    {
        public CancerCompVomitingDef()
        {
            Init(typeof(CancerCompVomiting));
        }

        public float mtbDays;
    }

    public class CancerCompVomiting : CancerComp<CancerCompVomitingDef>
    {
        public override void Update(int passed) => cancer.stage.vomitMtbDays = def.mtbDays;
    }

    // death
    public class CancerCompDeathDef : CancerCompDef
    {
        public CancerCompDeathDef()
        {
            Init(typeof(CancerCompDeath));
        }

        public float lethalSeverity;
    }

    public class CancerCompDeath : CancerComp<CancerCompDeathDef>
    {
        public override void Update(int passed) => cancer.lethalSeverity = def.lethalSeverity;
    }
}
