using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    public class Mutation :HediffWithComps
    {
        public new HediffMutationDef def => base.def as HediffMutationDef;
    }
}
