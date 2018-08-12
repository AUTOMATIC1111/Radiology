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

            Scribe_Values.Look(ref normal, "normal", 0f, false);
            Scribe_Values.Look(ref rare, "rare", 0f, false);
            Scribe_Values.Look(ref burn, "burn", 0f, false);
        }

        public override string TipStringExtra
        {
            get
            {
                return base.TipStringExtra +
                    "Burn (debug): " + burn + "\n" +
                    "Normal (debug): " + normal + "\n" +
                    "Rare (debug): " + rare + "\n";
            }
        }

        public override void Tick()
        {
            base.Tick();

            burn -= 0.001f;
            if (burn < 0) burn = 0;
        }

        public float burn;
        public float normal;
        public float rare;
    }
}
