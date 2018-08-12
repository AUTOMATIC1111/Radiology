using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    public interface IRadiationModifier
    {
        void Modify(ref RadiationInfo info);
    }
}
