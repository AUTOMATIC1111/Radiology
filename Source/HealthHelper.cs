using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    public static class HealthHelper
    {
        public static bool IsParent(BodyPartRecord potentialParent, BodyPartRecord potentialChild)
        {
            while (true)
            {
                Log.Message(potentialChild + " <-> " + potentialParent);

                if (potentialChild == null) return false;
                if (potentialChild == potentialParent) return true;

                potentialChild = potentialChild.parent;
            }
        }

    }
}
