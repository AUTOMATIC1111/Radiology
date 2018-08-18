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
        public int setTo;
    }

    public class MutationSetSkillDef : MutationDef
    {
        public MutationSetSkillDef() { hediffClass = typeof(MutationSetSkill); }

        public List<MutationSetSkillRecord> skills;
    }

    public class MutationSetSkill : Mutation<MutationSetSkillDef>
    {

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);

            foreach(var v in def.skills)
            {
                SkillRecord rec = pawn.skills.GetSkill(v.skill);
                if (rec == null) return;

                if (v.setTo != -1)
                {
                    rec.Level = v.setTo;
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

            return rec.setTo == -1;
        }
    }

}
