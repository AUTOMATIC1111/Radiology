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
        public float weight = 1.0f;

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

        public virtual object[] DescriptionArgs => null;

        public virtual bool IsValid() => true;

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
