using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    public static class MutationsHelper
    {
        public static List<HediffMutationDef> Mutations
        {
            get
            {
                if (allMutations != null) return allMutations;

                allMutations = new List<HediffMutationDef>();
                allMutations.AddRange(GenDefDatabase.GetAllDefsInDatabaseForDef(typeof(HediffMutationDef)).Cast<HediffMutationDef>());
                return allMutations;
            }
        }

        static List<HediffMutationDef> allMutations;
    }
}
