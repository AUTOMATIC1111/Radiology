using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Radiology
{
    public class Chamber : Building, ISelectMultiple<Pawn>, IRadiationReciever
    {
        new public ChamberDef def => base.def as ChamberDef;

        public Building Building => this;

        public string CanIrradiateNow(Pawn pawn = null)
        {
            if (powerComp == null || !powerComp.PowerOn)
                return "ChamberNoPower";

            if (pawn != currentUser && ticksCooldown > 0)
                return "ChamberCooldown";

            Room room = Position.GetRoom(Find.CurrentMap);
            if (room == null || room.PsychologicallyOutdoors)
                return "ChamberNoRoom";

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
                return intermediateProblem ?? "ChamberNoIrradiator";

            if (pawn != null && !IsHealthyEnoughForIrradiation(pawn))
            {
                return "ChamberPawnIsHurt";
            }

            if (pawn != null && !assigned.Contains(pawn))
            {
                return "ChamberPawnNotAssigned";
            }

            if (pawn != null && pawn.apparel.WornApparel.OfType<ShieldBelt>().Count() > 0)
            {
                return "ChamberShieldbelt";
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

        public override void Tick()
        {
            base.Tick();

            if (ticksCooldown > 0) ticksCooldown--;
        }

        public BodyPartRecord GetBodyPart(Pawn pawn)
        {
            if (pawn == null) return null;

            BodyPartRecord res = null;
            pawn.health.hediffSet.GetNotMissingParts().TryRandomElementByWeight(x => def.GetPartWeight(pawn, x), out res);
            return res;
        }

        public void Irradiate(Pawn pawn, int ticksCooldown)
        {
            this.ticksCooldown = ticksCooldown;
            currentUser = pawn;

            radiationTracker.Clear();

            Room room = Position.GetRoom(Map);
            Pawn actualPawn = Map.mapPawns.AllPawns.Where(x => x.GetRoom() == room).RandomElementWithFallback(pawn);

            foreach (CompIrradiator comp in GetIrradiators())
            {
                RadiationInfo info = new RadiationInfo { chamberDef = def, pawn = actualPawn, part = GetBodyPart(actualPawn), secondHand = actualPawn!=pawn, visited = new HashSet<CompIrradiator>() };
                comp.Irradiate(this, info, ticksCooldown);
                radiationTracker.burn += info.burn;
                radiationTracker.normal += info.normal;
                radiationTracker.rare += info.rare;

                if (actualPawn.IsShielded()) continue;

                HediffRadiation radiation = RadiationHelper.GetHediffRadition(info.part, info.pawn);
                if (radiation == null) continue;

                radiation.burn += info.burn;
                radiation.normal += info.normal;
                radiation.rare += info.rare;

                ApplyBurn(actualPawn, radiation);
                ApplyMutation(actualPawn, radiation);
            }
        }


        public void ApplyBurn(Pawn pawn, HediffRadiation radiation)
        {
            float burnAmount = def.burnThreshold.RandomInRange;
            if (radiation.burn < burnAmount) return;

            radiation.burn -= burnAmount;

            DamageInfo dinfo = new DamageInfo(DamageDefOf.Burn, burnAmount, 999999f, -1f, this, radiation.Part, null, DamageInfo.SourceCategory.ThingOrUnknown, null);
            pawn.TakeDamage(dinfo);

            RadiologyEffectSpawnerDef.Spawn(def.burnEffect, pawn);
        }

        public void ApplyMutation(Pawn pawn, HediffRadiation radiation)
        {
            if (radiation.normal + radiation.rare <= def.mutateThreshold.RandomInRange) return;

            float ratio = radiation.rare / (radiation.normal + radiation.rare);
            radiation.rare = 0;
            radiation.normal = 0;
            
            SpawnMutation(pawn, radiation.Part, ratio, radiation);
        }

        public void SpawnMutation(Pawn pawn, BodyPartRecord part, float ratio, HediffRadiation radiation = null)
        {
            Mutation mutation;
            var mutatedParts = RadiationHelper.MutatePawn(pawn, part, ratio, out mutation);
            lastMutation = mutation.def;
            lastMutationTick = Find.TickManager.TicksGame;
            if (mutatedParts == null) return;

            foreach (var anotherRadiation in pawn.health.hediffSet.GetHediffs<HediffRadiation>())
            {
                if (mutatedParts.Contains(anotherRadiation.Part) && radiation != anotherRadiation)
                {
                    anotherRadiation.normal -= def.mutateThreshold.min * (1f - ratio);
                    anotherRadiation.rare -= def.mutateThreshold.min * ratio;
                }
            }
        }


        public bool IsHealthyEnoughForIrradiation(Pawn pawn)
        {
            var pawnParts = pawn.health.hediffSet.GetNotMissingParts();
            var parts = def.bodyParts.Join(pawnParts, left => left.part, right => right.def, (left, right) => right);

            foreach (var part in parts)
            {
                float health = PawnCapacityUtility.CalculatePartEfficiency(pawn.health.hediffSet, part, false, null);
                if (health < damageThreshold) return false;
            }

            return true;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look(ref assigned, "assigned", LookMode.Reference);
            Scribe_Values.Look(ref ticksCooldown, "ticksCooldown");
            Scribe_Values.Look(ref damageThreshold, "damageThreshold");
            Scribe_References.Look(ref currentUser, "currentUser");
            Scribe_Defs.Look(ref lastMutation, "lastMutation");
            Scribe_Values.Look(ref lastMutationTick, "lastMutationTick");
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
                disabled = ticksCooldown > 0,
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
                    Find.WindowStack.Add(new DialogSelectMultiple<Pawn>(this)
                    {
                        LabelSelected = "ChamberDialogAssigned",
                        LabelNotSelected = "ChamberDialogNotAssigned",
                    });
                },
                hotKey = KeyBindingDefOf.Misc3
            };

            
            if (Prefs.DevMode && currentUser != null && currentUser.Spawned)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Dev: mutate "+ currentUser.Name,
                    action = delegate ()
                    {
                        SpawnMutation(currentUser, GetBodyPart(currentUser), 0.5f);
                    },
                };
            }


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
        
        public IEnumerable<Pawn> All() => Spawned ? Map.mapPawns.FreeColonistsAndPrisonersSpawned : Enumerable.Empty<Pawn>();
        string ISelectMultiple<Pawn>.Label(Pawn obj) => (obj.IsPrisoner ? "Prisoner: " : "Colonist: ") + obj.LabelCap;
        public bool IsSelected(Pawn obj) => assigned.Contains(obj);
        public void Select(Pawn obj) => assigned.Add(obj);
        public void Unselect(Pawn obj) => assigned.Remove(obj);

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

        public int ticksCooldown = 0;
        public float damageThreshold = 0.5f;
        public Pawn currentUser;


        public RadiationTracker radiationTracker = new RadiationTracker();
        public MutationDef lastMutation;
        public int lastMutationTick;

        private CompAffectedByFacilities facilitiesComp;
        private CompPowerTrader powerComp;
    }
}
