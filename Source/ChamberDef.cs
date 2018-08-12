using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    public class AffectedBodyPart
    {
        public BodyPartDef part;
    }

    public class ChamberDef : ThingDef
    {
        public FloatRange burnThreshold;
        public FloatRange mutateThreshold;
        public AutomaticEffectSpawnerDef burnEffect;

        public List<AffectedBodyPart> bodyParts;

        private Dictionary<BodyPartDef, float> cachedPartsMap;

        public Dictionary<BodyPartDef, float> PartsMap
        {
            get
            {
                if (cachedPartsMap != null) return cachedPartsMap;

                cachedPartsMap = new Dictionary<BodyPartDef, float>();
                if (bodyParts != null)
                {
                    foreach (AffectedBodyPart x in bodyParts)
                    {
                        cachedPartsMap[x.part] = x.part.hitPoints;
                    }
                }
                return cachedPartsMap;
            }
        }
    }
}
