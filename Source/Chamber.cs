using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Radiology
{
    public class Chamber : Building, IAssignableBuilding
    {

        new public ChamberDef def => base.def as ChamberDef;

        public static string causeNoPower = "ChamberNoPower";
        public static string causeNoRoom = "ChamberNoRoom";
        public static string causePawnHurt = "ChamberPawnIsHurt";
        public static string causeNoIrradiator = "ChamberNoIrradiator";
        public static string causePawnNotAssigned = "ChamberPawnNotAssigned";

        public string CanIrradiateNow(Pawn pawn = null)
        {
            if (powerComp == null || !powerComp.PowerOn)
                return causeNoPower;

            Room room = Position.GetRoom(Find.CurrentMap, RegionType.Set_All);
            if (room == null || room.PsychologicallyOutdoors)
                return causeNoRoom;

            int count = 0;
            string intermediateProblem = null;
            foreach (CompIrradiator comp in GetIrradiators())
            {
                string problem = comp.CanIrradiateNow(pawn);
                if (problem == null)
                    count++;
                else
                    intermediateProblem = problem;
            }

            if(count==0)
                return intermediateProblem ?? causeNoIrradiator;

            if (pawn != null && !IsHealthyEnoughForIrradiation(pawn))
            {
                return causePawnHurt;
            }

            if (pawn != null && !assigned.Contains(pawn))
            {
                return causePawnNotAssigned;
            }

            return null;
        }

        IEnumerable<CompIrradiator> GetIrradiators()
        {
            foreach (var v in facilitiesComp.LinkedFacilitiesListForReading)
            {
                ThingWithComps thing = v as ThingWithComps;
                if (thing == null) continue;

                foreach (CompIrradiator comp in thing.GetComps<CompIrradiator>())
                {
                    yield return comp;
                }
            }

            yield break;
        }

        public void Irradiate(Pawn pawn, int ticksCooldown)
        {
            radiationTracker.Clear();
            foreach (CompIrradiator comp in GetIrradiators())
            {
                RadiationInfo info = new RadiationInfo { chamber = this, pawn = pawn, part = null };
                comp.Irradiate(info, ticksCooldown);

                radiationTracker.burn += info.burn;
                radiationTracker.normal += info.normal;
                radiationTracker.rare += info.rare;
            }
        }

        public bool IsHealthyEnoughForIrradiation(Pawn pawn)
        {
            foreach (CompIrradiator comp in GetIrradiators())
            {
                if (!comp.IsHealthyEnoughForIrradiation(this, pawn)) return false;
            }

            return true;
        }


        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look(ref assigned, "assigned", LookMode.Reference);
        }


        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            powerComp = GetComp<CompPowerTrader>();
            facilitiesComp = GetComp<CompAffectedByFacilities>();

        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo g in base.GetGizmos())
            {
                yield return g;
            }

            if (Faction != Faction.OfPlayer)
                yield break;

            yield return new Command_Action
            {
                defaultLabel = "ChamberTestRunLabel".Translate(),
                defaultDesc = "ChamberTestRunDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("Radiology/Icons/TestRun", true),
                action = delegate ()
                {
                    Irradiate(null, 60);
                },
                hotKey = KeyBindingDefOf.Misc3
            };

            yield return new Command_Action
            {
                defaultLabel = "CommandRadiologyAssignChamberLabel".Translate(),
                defaultDesc = "CommandRadiologyAssignChamberDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/AssignOwner", true),
                action = delegate ()
                {
                    Find.WindowStack.Add(new Dialog_AssignBuildingOwner(this));
                },
                hotKey = KeyBindingDefOf.Misc3
            };

            yield break;
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder().Append(base.GetInspectString());

            string problem = CanIrradiateNow();
            if (problem != null)
            {
                if (stringBuilder.Length != 0)
                    stringBuilder.AppendLine();

                stringBuilder.Append(string.Format("ChamberUnavailable".Translate(), problem.Translate()));
            }

            return stringBuilder.ToString();
        }

        public HashSet<Pawn> assigned = new HashSet<Pawn>();

        public IEnumerable<Pawn> AssigningCandidates => Spawned ? Map.mapPawns.FreeColonists : Enumerable.Empty<Pawn>();

        public IEnumerable<Pawn> AssignedPawns => assigned;

        public int MaxAssignedPawnsCount => 99999;

        public void TryAssignPawn(Pawn pawn) => assigned.Add(pawn);

        public void TryUnassignPawn(Pawn pawn) => assigned.Remove(pawn);

        public bool AssignedAnything(Pawn pawn) => assigned.Contains(pawn);

        public class RadiationTracker
        {
            public float burn = 0;
            public float normal = 0;
            public float rare = 0;

            public void Clear()
            {
                burn = 0;
                normal = 0;
                rare = 0;
            }
        };

        public RadiationTracker radiationTracker = new RadiationTracker();

        private CompAffectedByFacilities facilitiesComp;
        private CompPowerTrader powerComp;
    }
}
