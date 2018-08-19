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
        public RadiologyEffectSpawnerDef burnEffect;

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

        public float GetPartWeight(Pawn pawn, BodyPartRecord x)
        {
            /// XXX can be optimized
            if (x.def.IsSolid(x, pawn.health.hediffSet.hediffs)) return 0f;

            return PartsMap.TryGetValue(x.def, 0f);
        }
    }
}
