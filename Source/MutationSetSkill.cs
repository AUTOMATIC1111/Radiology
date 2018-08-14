using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    public class MutationSetSkillDef : HediffMutationDef
    {
        public MutationSetSkillDef() { hediffClass = typeof(MutationSetSkill); }

        public SkillDef skill;
        public int setTo;
    }

    public class MutationSetSkill : Mutation<MutationSetSkillDef>
    {
        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref storedLevel, "storedLevel");
        }


        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);

            SkillRecord rec = pawn.skills.GetSkill(def.skill);
            if (rec == null) return;

            storedLevel = rec.Level;
            rec.Level = def.setTo;
        }

        public override void PostRemoved()
        {
            base.PostRemoved();

            SkillRecord rec = pawn.skills.GetSkill(def.skill);
            if (rec == null) return;

            if (storedLevel != -1) rec.Level = storedLevel;
        }

        public int storedLevel = -1;
    }

}
