using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Radiology
{
    class MoteRadiation : MoteThrown
    {
        internal List<float> reflectAt;
        internal bool isHorizontal;
        internal int reflectIndex;
        internal int reflectIndexChange;
        internal float reflectChance;
        internal float deathLocation;

        void Kill()
        {
            spawnTick = Find.TickManager.TicksGame - (int)(def.mote.Lifespan * 60) + 30;
        }

        protected override Vector3 NextExactPosition(float deltaTime)
        {
            Vector3 result = exactPosition + velocity * deltaTime;

            if (isHorizontal && !MathHelper.IsSameSign(exactPosition.x - deathLocation, result.x - deathLocation))
            {
                Kill();
                return result;
            }

            if (! isHorizontal && !MathHelper.IsSameSign(exactPosition.z - deathLocation, result.z - deathLocation))
            {
                Kill();
                return result;
            }

            bool reflect = false;
            if (reflectAt != null && reflectIndex >= 0 && reflectIndex < reflectAt.Count)
            {
                float coord = reflectAt[reflectIndex];
                if (isHorizontal)
                {
                    if (!MathHelper.IsSameSign(exactPosition.x - coord, result.x - coord))
                    {
                        reflectIndex += reflectIndexChange;
                        if (Rand.Range(0f, 1f) < reflectChance)
                        {
                            velocity.x = -velocity.x;
                            reflect = true;
                        }
                    }
                }
                else
                {
                    if (!MathHelper.IsSameSign(exactPosition.z-coord, result.z-coord))
                    {
                        reflectIndex += reflectIndexChange;
                        if (Rand.Range(0f, 1f) < reflectChance)
                        {
                            velocity.z = -velocity.z;
                            reflect = true;
                        }
                    }
                }
            }

            if (reflect)
            {
                result = exactPosition + velocity * deltaTime;
                exactRotation = velocity.ToAngleFlat() + 90;
                Kill();
            }

            return result;
        }

    }
}
