using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Verse
{
    public class RadiologyCancerCompDef : Def
    {
        public Type compClass;
        public string tag;

        public void Init(Type t)
        {
            compClass = t;
            tag = t.Name;
        }
    }
}

namespace Radiology
{

    public class CancerCompDef : RadiologyCancerCompDef
    {
    }

    public abstract class CancerComp : IExposable
    {
        public CancerCompDef def;

        public abstract object[] DescriptionArgs { get; }

        public abstract void Update(int passed);

        public void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
        }

        public Cancer cancer;
    }

    public abstract class CancerComp<T> : CancerComp where T : CancerCompDef
    {
        new public T def => base.def as T;
    }
}
