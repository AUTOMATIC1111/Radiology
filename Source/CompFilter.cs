using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Radiology
{
    class CompFilter :ThingComp, IRadiationModifier, ISelectMultiple<CompFilterOperationMode>
    {
        public new CompPropertiesFilter props => base.props as CompPropertiesFilter;

        public void Modify(ref RadiationInfo info)
        {
            CompFilterOperationMode m = Mode;
            if (m == null) return;

            m.Modify(ref info);
        }

        public override string CompInspectStringExtra()
        {
            CompFilterOperationMode m = Mode;
            string label = m == null ? "RadiologyFilterOperationModeNone".Translate().RawText : m.Label;
            return string.Format("RadiologyFilterOperationMode".Translate(), label);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (parent.Faction != Faction.OfPlayer) yield break;

            yield return new Command_Action
            {
                defaultLabel = "RadiologyFilterOperationModeLabel".Translate(),
                defaultDesc = "RadiologyFilterOperationModeDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("Radiology/Icons/SetupFiltering", true),
                action = delegate ()
                {
                    Find.WindowStack.Add(new DialogSelectMultiple<CompFilterOperationMode>(this)
                    {
                        LabelSelected = "RadiologyFilterOperationModeSelected",
                        LabelNotSelected = "RadiologyFilterOperationModeNotSelected",
                        SelectedLimit = 1,
                    });
                },
                hotKey = KeyBindingDefOf.Misc3
            };

            yield break;
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref mode, "mode", -1);
        }

        public IEnumerable<CompFilterOperationMode> All() => operationModes.Where( x => x.Available );

        public string Label(CompFilterOperationMode obj) => obj.Label.CapitalizeFirst();

        public bool IsSelected(CompFilterOperationMode obj) => Mode == obj;

        public void Select(CompFilterOperationMode obj) => mode = operationModes.FirstIndexOf(x => x == obj);

        public void Unselect(CompFilterOperationMode obj) => mode = -1;

        CompFilterOperationMode Mode => (mode >= 0 && mode < operationModes.Length) ? operationModes[mode] : null;

        int mode = -1;

        public static CompFilterOperationMode[] operationModes = {
            new CompFilterOperationModeBasic(),
            new CompFilterOperationModeIntermediate(),
            new CompFilterOperationModeAdvanced(),
            new CompFilterOperationModeRemoveNormal(),
            new CompFilterOperationModeRemoveNormalIntermediate(),
            new CompFilterOperationModeRemoveNormalAdvanced(),
            new CompFilterOperationModeTradeoffRemoveBurn(),
            new CompFilterOperationModeTradeoffRemoveNormal(),
            new CompFilterOperationModeDouble(),
        };
    }
}
