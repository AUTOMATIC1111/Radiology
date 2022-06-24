using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Radiology
{
    public static class ThingHelper
    {
        public static Vector3 ExactPosition(this Thing thing)
        {
            float x,z;

            if (thing.def.hasInteractionCell)
            {
                IntVec3 cell = thing.InteractionCell;
                x = 0.5f + cell.x;
                z = 0.5f + cell.z;
            }
            else
            {
                IntVec2 size = thing.def.size;
                IntVec3 cell = thing.Position;
                Rot4 rot = thing.Rotation;
                
                x = cell.x;
                z = cell.z;

                if (rot == Rot4.North)
                {
                    x += 0.5f * size.x;
                    z += 0.5f * size.z;
                }
                else if (rot == Rot4.South)
                {
                    x += 0.5f * size.x;
                    z -= -0.5f + 0.5f * size.z;
                }
                else if (rot == Rot4.East)
                {
                    x += 0.5f * size.z;
                    z += 0.5f * size.x;
                }
                else if (rot == Rot4.West)
                {
                    x -= -0.5f + 0.5f * size.z;
                    z += 0.5f * size.x;
                }
            }


            return new Vector3(x, thing.def.Altitude, z);
        }

        public static Vector3 RandomPointNearTrueCenter(this Thing t)
        {
            Vector3 res = t.TrueCenter();
            res.x += Rand.Range(-0.5f, 0.5f);
            res.z += Rand.Range(-0.5f, 0.5f);
            return res;
        }
    }
}
