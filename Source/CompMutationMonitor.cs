using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Radiology
{
    class CompMutationMonitor : ThingComp
    {
        Chamber chamber;

        void updateValues()
        {
            chamber = parent.Linked<Chamber>();
        }

        public override string CompInspectStringExtra()
        {
            updateValues();
            if (chamber?.lastMutation == null || chamber.lastMutationTick == 0) return "";
            
            return "RadiologyLastMutation".Translate(chamber.lastMutation.LabelCap, GenDate.ToStringTicksToPeriod(Find.TickManager.TicksGame - chamber.lastMutationTick));
        }

        public override void PostDraw()
        {
            updateValues();
            if (chamber?.lastMutation == null) return;

            Vector3 vec = parent.ExactPosition();
            vec.x += -dx; vec.y += 1; vec.z += -dz;

            int elasped = Find.TickManager.TicksGame - chamber.lastMutationTick;
            float colorAmount;

            if (elasped < durationFlashing + durationFadeoff)
            {
                float add = 0.5f;
                float mult = 0.5f;
                if(elasped > durationFlashing)
                {
                    float v = 1f - 1f * (elasped - durationFlashing) / durationFlashing;
                    mult = mult * v;
                }
                float x = (Mathf.Sin(elasped / 6f) + 1) * 0.5f;
                colorAmount = add + mult * x;
            }
            else
            {
                colorAmount = 0.5f;
            }

            Color baseColor = chamber.lastMutation.isBad ? colorBad : colorGood;

            DrawHelper.DrawMesh(vec, 0, scale, chamber.lastMutation.icon.Graphic.MatSingle, DrawHelper.Mix(baseColor, colorInactive, colorAmount));
        }

        Color colorGood = new ColorInt(79, 187, 37).ToColor;
        Color colorBad = new ColorInt(178, 80, 8).ToColor;
        Color colorInactive = new ColorInt(141, 141, 141).ToColor;

        int durationFlashing = 60 * 5;
        int durationFadeoff = 60 * 3;

        public static float dx = 0f / 64;
        public static float dz = -10f / 64;
        public static Vector3 scale = new Vector3(45f / 64, 1, 33f / 64);

    }
}
