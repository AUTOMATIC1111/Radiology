using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Radiology
{
    public class Command_Target_Location : Command
    {
        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            RimWorld.SoundDefOf.Tick_Tiny.PlayOneShotOnCamera(null);
            Find.Targeter.BeginTargeting(targetingParams, delegate (LocalTargetInfo target)
            {
                action(target);
            }, null, null, null);
        }

        public override bool InheritInteractionsFrom(Gizmo other)
        {
            return false;
        }

        public Action<LocalTargetInfo> action;

        public TargetingParameters targetingParams;
    }
}
