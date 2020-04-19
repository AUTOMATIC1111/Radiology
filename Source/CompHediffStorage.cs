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
        BodyPartDef part;
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

            for (int i = 0; i < hediffs.Count; i++)
            {
                Hediff hediff = hediffs[i];
                if (!hediff.Visible) continue;

                if (builder.Length != 0) builder.Append("\n");
                if (part != parts[i]) builder.Append(parts[i].LabelCap + ": ");
                builder.Append(hediffs[i].def.LabelCap);
            }

            return builder.ToString();
        }

        public override void PostPostMake()
        {
            if (!Radiology.itemBodyParts.TryGetValue(parent.def, out part)) return;

            // add a random mutation; if this item is spawned via operation, the mutation will be removed anyway
            MutationDef mutationDef = DefDatabase<MutationDef>.AllDefs.Where(x => x.affectedParts != null && x.affectedParts.Contains(part)).RandomElementWithFallback();

            Hediff hediff = (Hediff)Activator.CreateInstance(mutationDef.hediffClass);
            hediff.def = mutationDef;
            hediff.loadID = Find.UniqueIDsManager.GetNextHediffID();
            hediff.PostMake();

            hediffs.Add(hediff);
            parts.Add(part);
        }

        public IEnumerable<DefHyperlink> hyperlinks()
        {
            foreach (Hediff hediff in hediffs)
            {
                if (!hediff.Visible) continue;

                yield return hediff.def;
            }

            yield break;
        }
    }
}
