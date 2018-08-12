using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    // death
    public class CancerCompSpreadsDef : CancerCompDef
    {
        public CancerCompSpreadsDef()
        {
            Init(typeof(CancerCompSpreads));
        }

        public bool growUpwards = false;
        public bool growDownwards = false;
        public bool mutates = false;
        public float mtthDays = 1f;
    }

    public class CancerCompSpreads : CancerComp<CancerCompSpreadsDef>
    {
        public override bool IsValid()
        {
            bool growUpwards = def.growUpwards;
            bool growDownwards = def.growDownwards;

            if (cancer.Part.parent == null) growUpwards = false;
            if (! cancer.Part.GetDirectChildParts().Any()) growDownwards = false;

            if (!growUpwards && !growDownwards) return false;

            return true;
        }

        public IEnumerable<BodyPartRecord> PotentialTargetParts()
        {
            if (cancer.Part.parent != null && def.growUpwards)
            {
                yield return cancer.Part.parent;
            }

            foreach (BodyPartRecord childPart in cancer.Part.GetDirectChildParts())
            {
                yield return childPart;
            }

            yield break;
        }


        public IEnumerable<BodyPartRecord> TargetParts()
        {
            foreach(BodyPartRecord part in PotentialTargetParts())
            {
                if (part == null) continue;

                Cancer otherCancer = Cancer.GetCancerAt(cancer.pawn, part);
                if (otherCancer == null) yield return part;
            }

            yield break;
        }

        public override void Update(int passed)
        {
            if (passed < Rand.Range(0,(int)(def.mtthDays * 60000))) return;

            BodyPartRecord part = TargetParts().RandomElementWithFallback();
            if (part == null) return;

            Cancer newCancer = HediffMaker.MakeHediff(cancer.def, cancer.pawn, part) as Cancer;
            if (newCancer == null) return;

            if(! def.mutates)
                newCancer.BecomeCopyOf(cancer);

            cancer.pawn.health.AddHediff(newCancer, part, null, null);
        }
    }
}
