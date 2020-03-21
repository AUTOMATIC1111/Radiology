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
            Log.Message("=================");
            Log.Message("Find if "+ potentialChild + " is a child of "+ potentialParent);
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
