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

        public static Cancer GetCancerAt(Pawn pawn, BodyPartRecord part)
        {
            if (part == null) return null;
            return pawn.health.hediffSet.GetHediffs<Cancer>().Where(x => x.Part == part).FirstOrDefault();
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref diagnosed, "diagnosed");
            Scribe_Values.Look(ref apparentGrowth, "apparentGrowth");
            Scribe_Collections.Look(ref apparentComps, "apparentComps", LookMode.Deep);
            Scribe_Collections.Look(ref actualComps, "actualComps", LookMode.Deep);

            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                InitializeComps(apparentComps);
                InitializeComps(actualComps);
            }
            
            UpdateStage();
        }

        public void BecomeCopyOf(Cancer other)
        {
            diagnosed = other.diagnosed;
            apparentGrowth = other.apparentGrowth;
            Severity = other.Severity;

            foreach (var comp in other.apparentComps) apparentComps.Add(comp.CreateCopy());
            foreach (var comp in other.actualComps) actualComps.Add(comp.CreateCopy());

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
            apparentGrowth = false;
            diagnosed = false;
            actualComps.Clear();
            apparentComps.Clear();

            var list = def.symptomsPossible.Select(x => x);

            int count = def.symptomsCount.Lerped(Rand.Value * Rand.Value);
            for (int i = 0; i < count; i++)
            {
                if (!list.Any()) break;

                CancerCompDef def = list.RandomElementByWeight(x => x.weight);
                CancerComp comp = CreateComp(def);
                if (comp == null) continue;
                if (! comp.IsValid()) continue;

                actualComps.Add(comp);

                list = list.Where(x => x.tag != def.tag);
             }
        }

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);

            if (!actualComps.Any())
            {
                CreateComps();
                Severity = def.initialSeverityRange.RandomInRange;
            }
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
            apparentComps.Clear();

            foreach (CancerComp comp in actualComps)
            {
                float score = def.diagnoseDifficulty.RandomInRange;
                bool mistake = score > quality;
                bool unsure = Math.Abs(score-quality) < def.diagnoseUnsureWindow;

                if (!mistake)
                {
                    CancerComp copy = comp.CreateCopy();
                    copy.doctorIsUnsure = unsure;

                    apparentComps.Add(comp);
                    apparentTags.Add(comp.def.tag);
                    continue;
                }

                // 50% do diagnose wrongly, 50% to not notice
                if (Rand.Value < 0.5f)
                {
                    CancerCompDef mistakenDef = def.symptomsPossible.OfType<CancerCompDef>().Where(x => !apparentTags.Contains(x.tag)).RandomElementWithFallback();
                    if (mistakenDef == null) continue;

                    CancerComp mistakenComp = CreateComp(mistakenDef);
                    if (mistakenComp == null) continue;

                    mistakenComp.doctorIsUnsure = unsure;
                    apparentComps.Add(mistakenComp);
                    apparentTags.Add(mistakenComp.def.tag);
                }
            }

            apparentGrowth = apparentTags.Contains("growthSpeed");
        }

        public override string LabelInBrackets => (!diagnosed && !pawn.Dead) ?
            "RadiologyCancerNotDiagnosed".Translate() :
            (apparentGrowth || pawn.Dead)? string.Format("{0}%", (int) (Severity * 100)) : null;

        public override string TipStringExtra
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();

                HashSet<CancerCompDef> actualCompsMap = null;

                bool isDead = pawn.Dead;
                if (isDead)
                {
                    actualCompsMap = new HashSet<CancerCompDef>();
                    foreach (CancerComp comp in actualComps)
                    {
                        actualCompsMap.Add(comp.def);
                    }
                }

                if (diagnosed || isDead)
                {
                    foreach (CancerComp comp in apparentComps)
                    {
                        object[] args = comp.DescriptionArgs;
                        stringBuilder.Append("- ");
                        stringBuilder.Append(args == null ? comp.def.description : string.Format(comp.def.description, args));
                        if(isDead)
                        {
                            if (actualCompsMap.Contains(comp.def))
                            {
                                actualCompsMap.Remove(comp.def);
                            }
                            else
                            {
                                stringBuilder.Append("RadiologyCancerMisdiagnosed".Translate());
                            }
                        }
                        else if (comp.doctorIsUnsure)
                        {
                            stringBuilder.Append("RadiologyCancerDoctorUnsure".Translate());
                        }
                        stringBuilder.AppendLine();
                    }
                }

                if (isDead)
                {
                    foreach (CancerComp comp in actualComps)
                    {
                        if (!actualCompsMap.Contains(comp.def)) continue;

                        object[] args = comp.DescriptionArgs;
                        stringBuilder.Append("- ");
                        stringBuilder.Append(args == null ? comp.def.description : string.Format(comp.def.description, args));
                        stringBuilder.Append("RadiologyCancerMissed".Translate());
                        stringBuilder.AppendLine();
                    }
                }
                else if (diagnosed && !apparentGrowth)
                {
                    stringBuilder.Append("- ");
                    stringBuilder.AppendLine("RadiologyCancerNotGrowing".Translate());
                }


                return stringBuilder.ToString();
            }
        }

        public override bool CauseDeathNow()
        {
            return lethalSeverity >= 0f && Severity >= lethalSeverity;
        }

        int lastUpdateTick = -1;
        int nextUpdateTick = 0;

        public List<CancerComp> apparentComps = new List<CancerComp>();
        public bool apparentGrowth = false;
        public List<CancerComp> actualComps = new List<CancerComp>();

        public float lethalSeverity = -1f;
        public bool diagnosed = false;

        internal HediffStage stage = new HediffStage();
    }
}
