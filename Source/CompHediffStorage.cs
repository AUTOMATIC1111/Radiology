using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Radiology
{
    public class CompHediffStorage : ThingComp, IDefHyperlinkLister
    {
        public List<Hediff> hediffs = new List<Hediff>();
        public List<BodyPartDef> parts = new List<BodyPartDef>();

        public override void PostExposeData()
        {
            Scribe_Collections.Look(ref hediffs, "hediffs", LookMode.Deep);
            Scribe_Collections.Look(ref parts, "parts", LookMode.Def);
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder builder = new StringBuilder();
            bool multipleParts = parts.Distinct().Count() > 1;

            for (int i = 0; i < hediffs.Count; i++)
            {
                if (i != 0) builder.Append("\n");
                if (multipleParts) builder.Append(parts[i].LabelCap + ": " + hediffs[i].def.label);
                else builder.Append(hediffs[i].def.label);
            }

            return builder.ToString();
        }

        public IEnumerable<DefHyperlink> hyperlinks()
        {
            foreach (Hediff hediff in hediffs) {
                yield return hediff.def;
            }

            yield break;
        }
    }
}
