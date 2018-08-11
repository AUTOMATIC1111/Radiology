using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    public class HediffRadiation :HediffWithComps
    {
        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look<float>(ref radiation, "radiation", 0f, false);
            Scribe_Values.Look<float>(ref radiationRare, "radiationRare", 0f, false);
            Scribe_Values.Look<float>(ref burning, "burning", 0f, false);
        }

        public override string TipStringExtra
        {
            get
            {
                return base.TipStringExtra +
                    "Radiation (debug): " + radiation + "\n" +
                    "radiationRare (debug): " + radiationRare + "\n" +
                    "Burning (debug): " + burning + "\n";
            }
        }

        public override void Tick()
        {
            base.Tick();

            burning -= 0.001f;
            if (burning < 0) burning = 0;
        }

        public float radiation;
        public float radiationRare;
        public float burning;
    }
}
