using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace Radiology
{
    [HarmonyPatch(typeof(PawnApparelGenerator), "Reset", new Type[] { }), StaticConstructorOnStartup]
    class PatchPawnApparelGenerator
    {
        static private FieldInfo allApparelPairsField = typeof(PawnApparelGenerator).GetField("allApparelPairs", BindingFlags.NonPublic | BindingFlags.Static);
        static void Postfix()
        {
            List<ThingStuffPair> list = allApparelPairsField.GetValue(null) as List<ThingStuffPair>;
            list.RemoveAll((ThingStuffPair x) => typeof(ApparelBodyPart).IsAssignableFrom(x.thing.thingClass));
        }
    }

    [HarmonyPatch(typeof(PawnApparelGenerator), "GenerateStartingApparelFor", new Type[] { typeof(Pawn), typeof(PawnGenerationRequest) }), StaticConstructorOnStartup]
    class PatchPawnApparelGeneratorGenerateStartingApparelFor
    {
        static bool reset = false;

        static bool Prefix()
        {
            if (reset) return true;
            reset = true;

            PawnApparelGenerator.Reset();

            return true;
        }
    }
}