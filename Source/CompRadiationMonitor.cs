using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Radiology
{
    [StaticConstructorOnStartup]
    class CompRadiationMonitor :ThingComp
    {
        void updateValues()
        {
            Chamber chamber = parent.Linked<Chamber>();
            if (chamber == null)
            {
                values[0] = values[0] * 0.9f;
                values[1] = values[1] * 0.9f;
                values[2] = values[2] * 0.9f;
                return;
            }

            var tr = chamber.radiationTracker;
            var sum = tr.burn + tr.radiation + tr.radiationRare;
            if (sum == 0) sum = 1;

            values[0] = tr.burn / sum;
            values[1] = tr.radiation / sum;
            values[2] = tr.radiationRare / sum;
        }

        public override string CompInspectStringExtra()
        {
            updateValues();

            return
                    string.Format("RadiologyRadiationBurn".Translate(), (int)(100 * values[0])) + "\n" +
                    string.Format("RadiologyRadiationNormal".Translate(), (int)(100 * values[1])) + "\n" +
                    string.Format("RadiologyRadiationRare".Translate(), (int)(100 * values[2]));
        }

        public override void PostDraw()
        {
            Chamber chamber = parent.Linked<Chamber>();
            if (chamber == null) return;

            updateValues();

            if (unfilledMat == null)
            {
                unfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(107/255f, 107/255f, 107/255f), false);
                filledMat = new Material[]{
                    SolidColorMaterials.SimpleSolidColorMaterial(new Color(255/255f, 110/255f, 26/255f), false),
                    SolidColorMaterials.SimpleSolidColorMaterial(new Color(29/255f, 179/255f, 139/255f), false),
                    SolidColorMaterials.SimpleSolidColorMaterial(new Color(169/255f, 58/255f, 255/255f), false),
                };
            }

            for (int i = 0; i < 3; i++)
            {
                GenDraw.FillableBarRequest r = default(GenDraw.FillableBarRequest);
                r.center = parent.DrawPos + new Vector3((-8f+14*i)/64,0.1f,-4f/64);
                r.size = barSize;
                r.fillPercent = values[i];
                r.filledMat = filledMat[i];
                r.unfilledMat = unfilledMat;
                r.margin = 0;
                r.rotation = Rot4.West;
                GenDraw.DrawFillableBar(r);
            }
        }

        private float[] values = { 0, 0, 0 };
 
        private static readonly Vector2 barSize = new Vector2(30f / 64, 12f / 64);

        public static Material unfilledMat;
        public static Material[] filledMat;
    }
}
