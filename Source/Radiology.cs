using Harmony;
using System.Reflection;
using UnityEngine;
using Verse;

namespace Radiology
{
    [StaticConstructorOnStartup]
    public static class Radiology
    {

        static Radiology()
        {
            var harmony = HarmonyInstance.Create("com.github.automatic1111.radiology");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
