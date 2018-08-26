using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiology
{
    public static class HelperPowerNet
    {

        public static float AvailablePower(this PowerNet powerNet)
        {
            if (powerNet == null) return 0;

            float availablePower = 0;
            foreach (var battery in powerNet.batteryComps)
            {
                availablePower += battery.StoredEnergy;
            }
            return availablePower;
        }
        public static bool Drain(this PowerNet powerNet, float amount)
        {
            if (amount <= 0) return true;
            if (AvailablePower(powerNet) < amount) return false;

            foreach (var battery in powerNet.batteryComps)
            {
                var drain = battery.StoredEnergy > amount ? amount : battery.StoredEnergy;
                battery.DrawPower(drain);
                amount -= drain;

                if (amount <= 0) break;
            }

            return true;
        }
    }
}
