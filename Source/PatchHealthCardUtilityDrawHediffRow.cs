using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Radiology
{
    /// <summary>
    /// Draw an info icon for mutations with their descriptions in tooltip
    /// </summary>
    [HarmonyPatch(typeof(HealthCardUtility), "DrawHediffRow", new Type[] { typeof(Rect), typeof(Pawn), typeof(IEnumerable<Hediff>), typeof(float) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Ref }), StaticConstructorOnStartup]
    public static class PatchHealthCardUtilityDrawHediffRow
    {
        private static readonly Texture2D icon = ContentFinder<Texture2D>.Get("Radiology/Icons/Info", true);

        static void Prefix(Rect rect, Pawn pawn, IEnumerable<Hediff> diffs, ref float curY)
        {
            if(diffs.OfType<Mutation>().FirstOrDefault()==null) return;

            float firstRowWidth = rect.width * 0.375f;
            Rect rectIcon = new Rect(firstRowWidth - icon.width - 4, curY + 1, icon.width, icon.height);
            GUI.DrawTexture(rectIcon, icon);
            TooltipHandler.TipRegion(rectIcon, () => Tooltip(diffs), (int)curY + 117857);
        }

        static string Tooltip(IEnumerable<Hediff> diffs)
        {
            StringBuilder tooltip = new StringBuilder();
            foreach (Mutation mutation in diffs.OfType<Mutation>())
            {
                tooltip.AppendLine(mutation.LabelCap);
                tooltip.AppendLine(mutation.def.description != null ? mutation.def.description : "RadiologyTooltipNoDescription".Translate());
                tooltip.AppendLine();
            }

            return tooltip.ToString();
        }
    }
}
