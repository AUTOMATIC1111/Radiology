using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse.Sound;
using Radiology;
using Verse;
using RimWorld;

namespace Verse
{
    public class RadiologyEffectSpawnerDef : Def
    {
        public FloatRange offset;
        public List<AutomaticSubEffect> subEffects;
        public List<AutomaticEffectSpawnerDef> alsoSpawn;
    }
}

namespace Radiology
{
    public abstract class AutomaticSubEffect
    {
        public abstract void Spawn(Map map, Vector3 position, float angle);
    }

    public class AutomaticSubEffectSound : AutomaticSubEffect
    {
        public SoundDef sound;

        public override void Spawn(Map map, Vector3 position, float angle)
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
        public float arc = 360f;

        public override void Spawn(Map map, Vector3 origin, float initialAngle)
        {
            for (int i = 0; i < moteCount; i++)
            {
                MoteThrown moteThrown = ThingMaker.MakeThing(mote, null) as MoteThrown;
                if (moteThrown == null) return;

                moteThrown.thingIDNumber = -1 - i;

                float angle = initialAngle + Rand.Range(- arc / 2, arc / 2);
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

    public class AutomaticEffectSpawnerDef : RadiologyEffectSpawnerDef
    {
        public static void Spawn(AutomaticEffectSpawnerDef effect, Map map, Vector3 position, float angle=0f)
        {
            if (effect != null)
                effect.Spawn(map, position, angle);
        }

        public static void Spawn(AutomaticEffectSpawnerDef effect, Pawn pawn, float angle = 0f)
        {
            Spawn(effect, pawn.Map, pawn.TrueCenter(), angle);
        }

        public void Spawn(Map map, Vector3 position, float angle)
        {
            Vector3 origin = position + new Vector3(1f, 0f, 0f).RotatedBy(Rand.Range(0f, 360f)) * offset.RandomInRange;

            if (subEffects != null)
            {
                foreach (var subEffect in subEffects)
                {
                    subEffect.Spawn(map, origin, angle);
                }
            }

            if (alsoSpawn != null)
            {
                foreach (var spawner in alsoSpawn)
                {
                    spawner.Spawn(map, origin, angle);
                }
            }
        }

    }
}
