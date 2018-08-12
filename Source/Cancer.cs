using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    public class Cancer : Mutation
    {
        new CancerDef def => base.def as CancerDef;

        public override void ExposeData()
        {
            base.ExposeData();
            
            Scribe_Values.Look(ref diagnosed, "diagnosed");
            Scribe_Collections.Look(ref apparentComps, "apparentComps", LookMode.Deep);
            Scribe_Collections.Look(ref actualComps, "actualComps", LookMode.Deep);

            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                InitializeComps(apparentComps);
                InitializeComps(actualComps);
            }
            
            UpdateStage();
        }

        private void InitializeComps(List<CancerComp> list)
        {
            foreach (CancerComp comp in list)
            {
                comp.cancer = this;
            }
        }

        public CancerComp CreateComp(CancerCompDef def)
        {
            CancerComp comp = Activator.CreateInstance(def.compClass) as CancerComp;
            if (comp == null) return null;

            comp.def = def;
            comp.cancer = this;

            return comp;
        }

        public void CreateComps()
        {
            HashSet<string> tagsUsed = new HashSet<string>();
            
            foreach (CancerCompDef def in def.defsPossible.OfType<CancerCompDef>())
            {
                if (Rand.Range(0f, 1f) < 0.5f) continue;
                if (tagsUsed.Contains(def.tag)) continue;

                CancerComp comp = CreateComp(def);
                if (comp == null) continue;

                actualComps.Add(comp);
                tagsUsed.Add(def.tag);
            }
        }

        public override void PostAdd(DamageInfo? dinfo)
        {
            CreateComps();
            
            Severity = def.initialSeverityRange.RandomInRange;
        }

        public override HediffStage CurStage
        {
            get
            {
                UpdateStage();

                return stage;
            }
        }

        private void UpdateStage()
        {
            int tick = Find.TickManager.TicksGame;
            if (tick < nextUpdateTick) return;

            int passed = lastUpdateTick == -1 ? 0 : tick - lastUpdateTick;
            lastUpdateTick = tick;
            nextUpdateTick = tick + 600;

            stage.capMods.Clear();
            foreach (CancerComp comp in actualComps.OfType<CancerComp>())
            {
                comp.Update(passed);
            }
        }

        public override bool TendableNow(bool ignoreTimer)
        {
            return !diagnosed;
        }
        public override void Tended(float quality, int batchPosition)
        {
            diagnosed = true;
            HashSet<string> apparentTags = new HashSet<string>();

            foreach (CancerComp comp in actualComps)
            {
                bool mistake = Rand.Range(0f, def.diagnoseDifficulty) > quality;

                if (!mistake)
                {
                    apparentComps.Add(comp);
                    apparentTags.Add(comp.def.tag);
                    continue;
                }

                // 50% do diagnose wrongly, 50% to not notice
                if (Rand.Range(0f, 1f) < 0.5f)
                {
                    CancerCompDef mistakenDef = def.defsPossible.OfType<CancerCompDef>().Where(x => !apparentTags.Contains(x.tag)).RandomElementWithFallback();
                    if (mistakenDef == null) continue;

                    CancerComp mistakenComp = CreateComp(mistakenDef);
                    if (mistakenComp == null) continue;

                    apparentComps.Add(mistakenComp);
                }
            }
            
        }


        public override string LabelInBrackets => string.Format("{0}%", (int) (Severity * 100));

        public override string TipStringExtra
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();

                if (diagnosed)
                {
                    bool hasGrowthSpeed = false;
                    foreach (CancerComp comp in apparentComps)
                    {
                        object[] args = comp.DescriptionArgs;
                        stringBuilder.Append("- ");
                        stringBuilder.AppendLine(args == null ? comp.def.description : string.Format(comp.def.description, args));

                        if (comp.def.tag == "growthSpeed") hasGrowthSpeed = true;
                    }

                    if (!hasGrowthSpeed)
                    {
                        stringBuilder.Append("- ");
                        stringBuilder.AppendLine("RadiologyCancerNotGrowing".Translate());
                    }
                }
                else
                {
                    stringBuilder.Append("- ");
                    stringBuilder.AppendLine("RadiologyCancerNotDiagnosed".Translate());
                }

                /*
                stringBuilder.AppendLine(" <<< Actual >>> (debug):");
                foreach (CancerComp comp in actualComps)
                {
                    object[] args = comp.DescriptionArgs;
                    stringBuilder.Append("- ");
                    stringBuilder.AppendLine(args == null ? comp.def.description : string.Format(comp.def.description, args));
                }
                */
                return stringBuilder.ToString();
            }
        }

        int lastUpdateTick = -1;
        int nextUpdateTick = 0;

        public List<CancerComp> apparentComps = new List<CancerComp>();
        public List<CancerComp> actualComps = new List<CancerComp>();

        public bool diagnosed = false;

        internal HediffStage stage = new HediffStage();
    }
}
