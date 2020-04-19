using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Radiology
{
    public class Mutcomp<T> : HediffComp where T : HediffCompProperties
    {
        public new T props => base.props as T;
    }

    public class MutcompProps<T> : HediffCompProperties where T : HediffComp
    {
        public MutcompProps() { compClass = typeof(T); }
    }

}
