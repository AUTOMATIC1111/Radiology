using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Radiology.Patch
{
    public static class PatchDefGeneratorGenerateImpliedDefs_PreResolve
    {
        static HashSet<String> ignoreParts = new HashSet<String>() { "Brain", "Head" };

        static void Postfix()
        {
            RecipeDef installHeart = DefDatabase<RecipeDef>.GetNamed("InstallNaturalHeart");

            foreach (BodyPartDef part in DefDatabase<MutationDef>.AllDefs.Where(x => x.affectedParts != null).SelectMany(x => x.affectedParts).Distinct())
            {
                ThingDef def = new ThingDef();
                def.isTechHediff = true;
                def.category = ThingCategory.Item;
                def.thingClass = typeof(ThingWithComps);
                def.thingCategories = new List<ThingCategoryDef>() { ThingCategoryDefOf.BodyParts };
                def.graphicData = new GraphicData() { graphicClass = typeof(Graphic_Single), texPath = "Radiology/Items/MutatedBodyPartBox" };
                def.uiIconPath = "Radiology/Items/MutatedBodyPartBox";
                def.useHitPoints = true;
                def.selectable = true;
                def.SetStatBaseValue(StatDefOf.MaxHitPoints, 50f);
                def.SetStatBaseValue(StatDefOf.Flammability, 0.7f);
                def.SetStatBaseValue(StatDefOf.MarketValue, 500f);
                def.SetStatBaseValue(StatDefOf.Mass, 1f);
                def.SetStatBaseValue(StatDefOf.SellPriceFactor, 1.0f);
                def.altitudeLayer = AltitudeLayer.Item;
                def.tickerType = TickerType.Never;
                def.alwaysHaulable = true;
                def.rotatable = false;
                def.pathCost = 15;
                def.drawGUIOverlay = true;
                def.modContentPack = Radiology.modContentPack;
                def.tradeability = Tradeability.None;
                def.defName = "Mutated" + part.defName;
                def.label = "Mutated " + part.label;
                def.description = "Mutated " + part.label;
                def.comps.Add(new CompProperties_Forbiddable());
                def.comps.Add(new CompProperties_Glower() { glowColor = new ColorInt(0, 255, 0), glowRadius = 1 });
                def.comps.Add(new CompProperties() { compClass = typeof(CompHediffStorage) });


                RecipeDef recipe = new RecipeDef();
                recipe.defName = "InstallMutated" + part.defName;
                recipe.label = "install mutated " + part.label;
                recipe.description = "Install a mutated " + part.label + ".";
                recipe.descriptionHyperlinks = new List<DefHyperlink>() { def };
                recipe.jobString = "Installing mutated " + part.label + ".";
                recipe.workerClass = typeof(Recipe_InstallMutatedBodyPart);
                recipe.targetsBodyPart = true;
                recipe.dontShowIfAnyIngredientMissing = true;

                recipe.effectWorking = installHeart.effectWorking;
                recipe.soundWorking = installHeart.soundWorking;
                recipe.workSpeedStat = installHeart.workSpeedStat;
                recipe.workSkill = installHeart.workSkill;
                recipe.workSkillLearnFactor = installHeart.workSkillLearnFactor;
                recipe.workAmount = installHeart.workAmount;
                recipe.anesthetize = installHeart.anesthetize;
                recipe.deathOnFailedSurgeryChance = installHeart.deathOnFailedSurgeryChance;

                recipe.surgerySuccessChanceFactor = 1.2f;
                recipe.modContentPack = Radiology.modContentPack;

                IngredientCount medicine = new IngredientCount();
                medicine.SetBaseCount(2);
                medicine.filter.SetAllow(RimWorld.ThingDefOf.MedicineIndustrial, true);
                recipe.ingredients.Add(medicine);

                IngredientCount mutatedPart = new IngredientCount();
                mutatedPart.filter.SetAllow(def, true);
                recipe.ingredients.Add(mutatedPart);

                recipe.recipeUsers = new List<ThingDef>(installHeart.recipeUsers);
                recipe.appliedOnFixedBodyParts = new List<BodyPartDef>() { part };
                recipe.skillRequirements = new List<SkillRequirement>() { new SkillRequirement() { minLevel = 8, skill = SkillDefOf.Medicine } };

                def.descriptionHyperlinks = new List<DefHyperlink>() { recipe };

                if (! ignoreParts.Contains(part.defName)) Radiology.bodyPartItems[part] = def;
                Radiology.itemBodyParts[def] = part;

                DefGenerator.AddImpliedDef(def);
                DefGenerator.AddImpliedDef(recipe);
            }
        }
    }
}
