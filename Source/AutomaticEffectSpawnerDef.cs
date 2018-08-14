using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse.Sound;

namespace Verse
{
    public abstract class AutomaticSubEffect
    {
        public abstract void Spawn(Map map, Vector3 position);
    }

    public class AutomaticSubEffectSound : AutomaticSubEffect
    {
        public SoundDef sound;

        public override void Spawn(Map map, Vector3 position)
        {
            sound.PlayOneShot(new TargetInfo(position.ToIntVec3(), map, false));
        }
    }

    public class AutomaticSubEffectRadial : AutomaticSubEffect
    {
        public ThingDef mote;
        public int moteCount;
        public FloatRange radius;
        public FloatRange scale;
        public float speed;

        public override void Spawn(Map map, Vector3 origin)
        {
            for (int i = 0; i < moteCount; i++)
            {
                MoteThrown moteThrown = ThingMaker.MakeThing(mote, null) as MoteThrown;
                if (moteThrown == null) return;

                moteThrown.thingIDNumber = -1 - i;

                float angle = Rand.Range(0f, 360f);
                Vector3 dir = new Vector3(1f, 0f, 0f).RotatedBy(angle);

                float magnitude = Rand.Range(0f, 1f);
                float actualScale = scale.LerpThroughRange(magnitude);
                moteThrown.exactPosition = origin + dir * radius.LerpThroughRange(magnitude);
                moteThrown.exactRotation = angle + 90;
                moteThrown.exactScale = new Vector3(actualScale, actualScale, actualScale);
                moteThrown.SetVelocity(angle + 90, magnitude * speed);
                GenSpawn.Spawn(moteThrown, origin.ToIntVec3(), map, WipeMode.Vanish);
                moteThrown.spawnTick -= (int)(magnitude * 0.75f * (mote.mote.fadeOutTime * 60));
            }
        }
    }

    public class AutomaticEffectSpawnerDef : Def
    {
        public void Spawn(Map map, Vector3 position)
        {
            Vector3 origin = position + new Vector3(1f, 0f, 0f).RotatedBy(Rand.Range(0f, 360f)) * offset.RandomInRange;

            if (subEffects != null)
            {
                foreach (var subEffect in subEffects)
                {
                    subEffect.Spawn(map, origin);
                }
            }

            if (alsoSpawn != null)
            {
                foreach (var spawner in alsoSpawn)
                {
                    spawner.Spawn(map, origin);
                }
            }
        }

        public FloatRange offset;
        public List<AutomaticSubEffect> subEffects;
        public List<AutomaticEffectSpawnerDef> alsoSpawn;
    }
}
