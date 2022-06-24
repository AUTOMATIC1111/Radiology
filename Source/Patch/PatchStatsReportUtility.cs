using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Radiology.Patch
{
    [HarmonyPatch(typeof(StatsReportUtility), "DescriptionEntry", new Type[] { typeof(Thing) })]
    class PatchStatsReportUtilityDescriptionEntry
    {
        static StatDrawEntry Postfix(StatDrawEntry entry, Thing thing)
        {
            ThingWithComps t = thing as ThingWithComps;
            if (t == null) return entry;

            IEnumerable<IDefHyperlinkLister> listers = t.Comps()?.OfType<IDefHyperlinkLister>();
            if (listers==null || listers.Count() == 0) return entry;

            var hyperlinks = Traverse.Create(entry).Field<IEnumerable<Dialog_InfoCard.Hyperlink>>("hyperlinks");
            if (hyperlinks != null)
            {
                hyperlinks.Value = Dialog_InfoCard.DefsToHyperlinks(listLinks(thing, listers));
            }

            return entry;
        }

        static IEnumerable<DefHyperlink> listLinks(Thing thing, IEnumerable<IDefHyperlinkLister> listers)
        {
            if (thing.def.descriptionHyperlinks != null) foreach (var l in thing.def.descriptionHyperlinks)
                    yield return l;

            foreach (var lister in listers)
                foreach (var l in lister.hyperlinks())
                    yield return l;

            yield break;
        }
    }
}
