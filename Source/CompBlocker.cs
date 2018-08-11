using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Radiology
{
    class CompBlocker : ThingComp, ISelectMultiple<BodyPartRecord>
    {
        BodyDef bodyDef = BodyDefOf.Human;

        public new CompPropertiesBlocker props => base.props as CompPropertiesBlocker;

        void assignAllParts(){
            allParts.Clear();

            List<Thing> irradiators = facility.LinkedBuildings();
            Building building = irradiators.FirstOrDefault(x => (x as Building).GetComp<CompIrradiator>() != null) as Building;
            if (building == null) return;

            CompIrradiator comp = building.GetComp<CompIrradiator>();
            var partsThatCanBeIrradiated = comp.props.bodyParts.Select(x => x.part);
            IEnumerable<BodyPartRecord> parts = bodyDef.AllParts.Where(x => partsThatCanBeIrradiated.Contains(x.def));

            foreach (var v in parts) allParts.Add(v);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (parent.Faction != Faction.OfPlayer) yield break;

            assignAllParts();
            if (!allParts.Any()) yield break;

            yield return new Command_Action
            {
                defaultLabel = "CommandRadiologyAssignRadiationBlockingLabel".Translate(),
                defaultDesc = "CommandRadiologyAssignRadiationBlockingDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("Radiology/Icons/SetupBlocking", true),
                action = delegate ()
                {
                    Find.WindowStack.Add(new DialogSelectMultiple<BodyPartRecord>(this) {
                        LabelSelected = "RadiologyFilterEnabled",
                        LabelNotSelected = "RadiologyFilterDisabled",
                        SelectedLimit = props.blockedBodyPartLimit,
                    });
                },
                hotKey = KeyBindingDefOf.Misc3
            };

            yield break;
        }

        public override string CompInspectStringExtra()
        {
            string list = string.Join(",", enabledParts.Select(x => x.Label).ToArray());
            if (list.Length == 0) list = "RadiologyFilterNone".Translate();
            return string.Format("RadiologyFilterBlocking".Translate(), list);
        }

        public override void PostExposeData()
        {
            Scribe_Collections.Look(ref enabledParts, "enabledParts", LookMode.BodyPart);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            powerComp = parent.TryGetComp<CompPowerTrader>();
            facility = parent.TryGetComp<CompFacility>();
        }

        public IEnumerable<BodyPartRecord> All() => allParts;
        public string Label(BodyPartRecord obj) => obj.LabelCap;
        public bool IsSelected(BodyPartRecord obj) => enabledParts.Contains(obj);
        public void Select(BodyPartRecord obj) => enabledParts.Add(obj);
        public void Unselect(BodyPartRecord obj) => enabledParts.Remove(obj);

        public HashSet<BodyPartRecord> enabledParts = new HashSet<BodyPartRecord>();
        public HashSet<BodyPartRecord> allParts = new HashSet<BodyPartRecord>();

        private CompFacility facility;
        private CompPowerTrader powerComp;
    }
}
