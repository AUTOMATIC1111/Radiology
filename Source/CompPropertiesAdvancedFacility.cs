using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Radiology
{
    public class CompPropertiesAdvancedFacility : CompProperties_Facility, IAdvancedFacilityConnector
    {
        public bool mustBeFacing = false;
        public bool mustBeFaced = false;


        static IntVec2 RotatedSize(ThingDef def, Rot4 rot)
        {
            if (!rot.IsHorizontal)
                return def.size;
            else
                return new IntVec2(def.size.z, def.size.x);
        }


        private void GetCoordinates(ThingDef def, IntVec3 pos, Rot4 rot, out int x1, out int z1, out int x2, out int z2)
        {
            x1 = pos.x;
            z1 = pos.z;
            x2 = x1;
            z2 = z1;

            int w, h;
            if (rot.IsHorizontal)
            {
                w = def.size.z;
                h = def.size.x;
            }
            else
            {
                w = def.size.x;
                h = def.size.z;
            }
            int w1 = w / 2, w2 = w - w1;
            int h1 = h / 2, h2 = h - h1;


            switch (rot.AsInt)
            {
                case 0:
                    x1 -= w2 - 1;
                    x2 += w1;
                    z1 -= h2 - 1;
                    z2 += h1;
                    break;
                case 1:
                    x1 -= w2 - 1;
                    x2 += w1;
                    z1 -= h1;
                    z2 += h2 - 1;
                    break;
                case 2:
                    x1 -= w1;
                    x2 += w2 - 1;
                    z1 -= h1;
                    z2 += h2 - 1;
                    break;
                case 3:
                    x1 -= w1;
                    x2 += w2 - 1;
                    z1 -= h2 - 1;
                    z2 += h1;
                    break;
            }

        }

        public bool CanLinkTo(bool baseResult, ThingDef facilityDef, IntVec3 facilityPos, Rot4 facilityRot, ThingDef myDef, IntVec3 myPos, Rot4 myRot)
        {
            if (!baseResult) return false;

            int mx1, mz1, mx2, mz2;
            GetCoordinates(myDef, myPos, myRot, out mx1, out mz1, out mx2, out mz2);

            int fx1, fz1, fx2, fz2;
            GetCoordinates(facilityDef, facilityPos, facilityRot, out fx1, out fz1, out fx2, out fz2);
            
            IntVec2 mySize = RotatedSize(myDef, myRot);
            IntVec2 facilitySize = RotatedSize(facilityDef, facilityRot);

            if (mustBeFacing)
            {
                Vector2 vec = facilityRot.AsVector2;
                int dx = (int)vec.x;
                int dy = (int)vec.y;

                if (dx != 0)
                {
                    if (!MathHelper.IsSameSign(dx, mx1 - fx1)) return false;
                    if (!MathHelper.IsBetween(fz1, mz1, mz2) && !MathHelper.IsBetween(fz2, mz1, mz2)) return false;

                }

                if (dy != 0)
                {
                    if (!MathHelper.IsSameSign(dy, mz1 - fz1)) return false;
                    if (!MathHelper.IsBetween(fx1, mx1, mx2) && !MathHelper.IsBetween(fx2, mx1, mx2)) return false;
                }
            }

            if (mustBeFaced)
            {
                Vector2 vec = myRot.AsVector2;
                int dx = (int)vec.x;
                int dy = (int)vec.y;

                if (dx != 0)
                {
                    if (!MathHelper.IsSameSign(dx, fx1-mx1)) return false;
                    if (!MathHelper.IsBetween(fz1, mz1, mz2) && !MathHelper.IsBetween(fz2, mz1, mz2)) return false;

                }

                if (dy != 0)
                {
                    if (!MathHelper.IsSameSign(dy, fz1-mz1)) return false;
                    if (!MathHelper.IsBetween(fx1, mx1, mx2) &&!MathHelper.IsBetween(fx2, mx1, mx2)) return false;
                }
            }

            return true;
        }

    }
}
