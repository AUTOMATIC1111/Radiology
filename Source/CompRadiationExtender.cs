using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Radiology
{
    class CompRadiationExtender : CompIrradiator, IRadiationReciever
    {
        public Building Building => parent as Building;
        public CompPropertiesIrradiator moteProps;

        IEnumerable<CompIrradiator> GetIrradiators()
        {
            foreach (var v in parent.GetComp<CompAffectedByFacilities>().LinkedFacilitiesListForReading)
            {
                ThingWithComps thing = v as ThingWithComps;
                if (thing == null) continue;

                foreach (CompIrradiator comp in thing.GetComps<CompIrradiator>())
                {
                    yield return comp;
                }
            }

            yield break;
        }

        public override void CreateRadiation(RadiationInfo fullInfo, int ticks)
        {
            foreach (CompIrradiator comp in GetIrradiators().InRandomOrder())
            {
                RadiationInfo info = new RadiationInfo();
                info.CopyFrom(fullInfo);
                comp.Irradiate(parent as Building, info, ticks);
                if (info.Empty()) continue;

                moteProps = comp.MoteProps;

                fullInfo.Add(info);
            }
        }

        public override CompPropertiesIrradiator MoteProps => moteProps;
    }
}
