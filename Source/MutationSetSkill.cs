using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    public class MutationSetSkillRecord
    {
        public SkillDef skill;
        public int setTo = -1;
        public int add = 0;
    }

    public class MutationSetSkillDef : MutationDef
    {
        public MutationSetSkillDef() { hediffClass = typeof(MutationSetSkill); }

        public List<MutationSetSkillRecord> skills;
    }

    public class MutationSetSkill : Mutation<MutationSetSkillDef>
    {
        public override string TipStringExtra
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(base.TipStringExtra);

                foreach (var v in def.skills)
                {
                    if (v.add != 0)
                    {
                        builder.AppendLine(string.Format("{0}: {1}", v.skill.LabelCap, v.add));
                    }
                    else if (v.setTo != -1)
                    {
                        builder.AppendLine(string.Format("{0}: {1}", v.skill.LabelCap, v.setTo));
                    }
                    else
                    {
                        builder.AppendLine(string.Format("{0}: {1}", v.skill.LabelCap, "RadiologyTooltipSkillDisabled".Translate()));
                    }
                }

                return builder.ToString();
            }
        }

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);

            foreach(var v in def.skills)
            {
                SkillRecord rec = pawn.skills.GetSkill(v.skill);
                if (rec == null) return;

                if (v.add != 0)
                {
                    rec.Level += v.add;
                }
                else if (v.setTo != -1)
                {
                    rec.Level = v.setTo;
                }
                else
                {
                    rec.Notify_SkillDisablesChanged();
                }
            }
        }

        public override void PostRemoved()
        {
            base.PostRemoved();

            foreach (var v in def.skills)
            {
                SkillRecord rec = pawn.skills.GetSkill(v.skill);
                if (rec == null) return;

                if (v.add != 0)
                {
                    rec.Level -= v.add;
                }
                else if (v.setTo != -1)
                {
                    rec.Level = 0;
                }
                else
                {
                    rec.Notify_SkillDisablesChanged();
                }
            }
        }

        public bool IsSkillDisabled(SkillDef skill)
        {
            MutationSetSkillRecord rec = def.skills.FirstOrDefault(x => x.skill == skill);
            if (rec == null) return false;

            return rec.setTo == -1 && rec.add == 0;
        }
    }
}
