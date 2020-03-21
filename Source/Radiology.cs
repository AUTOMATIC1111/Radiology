using HarmonyLib;
using Radiology.Patch;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace Radiology
{
    public class Radiology : Mod
    {
        public static Harmony harmony = new Harmony("com.github.automatic1111.radiology");
        public static ModContentPack modContentPack;
        public static Dictionary<BodyPartDef, ThingDef> bodyPartItems = new Dictionary<BodyPartDef, ThingDef>();

        public Radiology(ModContentPack pack) : base(pack)
        {
            modContentPack = pack;

            harmony.Patch(
                AccessTools.Method(typeof(DefGenerator), "GenerateImpliedDefs_PreResolve"),
                null, new HarmonyMethod(typeof(PatchDefGeneratorGenerateImpliedDefs_PreResolve), "Postfix")
            );
        }
    }

    [StaticConstructorOnStartup]
    public static class RadiologyPatch
    {
        static RadiologyPatch()
        {
            Radiology.harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
