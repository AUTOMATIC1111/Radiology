using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    public class RoomRoleWorker_RadiologyChamber : RoomRoleWorker
    {
        HashSet<Type> applicableTypes = new HashSet<Type>(new Type[] { typeof(Chamber), typeof(CompIrradiator), typeof(CompFilter), typeof(CompBlocker) });

        public override float GetScore(Room room)
        {
            float result = 0;

            foreach (ThingWithComps thing in room.ContainedAndAdjacentThings.OfType<ThingWithComps>())
            {
                if (applicableTypes.Contains(thing.GetType()))
                {
                    result += 1000;
                    continue;
                }

                foreach (var v in thing.GetComps<ThingComp>())
                {
                    if (applicableTypes.Contains(v.GetType()))
                    {
                        result += 10000;
                        continue;
                    }
                }
            }

            return result;
        }
    }
}
