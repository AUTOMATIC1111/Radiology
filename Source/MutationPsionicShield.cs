using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Radiology
{
    [StaticConstructorOnStartup]
    public class GizmoPsionicShieldStatus : Gizmo
    {
        static int ID = 10984688;

        public GizmoPsionicShieldStatus()
        {
            order = -200f;
        }

        public override float GetWidth(float maxWidth)
        {
            return 140f;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth)
        {
            Rect overRect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            Find.WindowStack.ImmediateWindow(ID, overRect, WindowLayer.GameUI, delegate
            {
                Rect rect = overRect.AtZero().ContractedBy(6f);

                Rect rectLabel = rect;
                rectLabel.height = overRect.height / 2f;
                Text.Font = GameFont.Tiny;
                Widgets.Label(rectLabel, mutation.LabelCap);

                Rect rectBar = rect;
                rectBar.yMin = overRect.height / 2f;
                float fillPercent = mutation.health / Mathf.Max(1f, mutation.def.health);
                Widgets.FillableBar(rectBar, fillPercent, FullShieldBarTex, EmptyShieldBarTex, false);
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rectBar, (mutation.health).ToString("F0") + " / " + (mutation.def.health).ToString("F0"));

                float rechargeFillPercent = mutation.regenerationDelay / Mathf.Max(1f, mutation.def.regenerationDelayTicks);
                if (rechargeFillPercent > 0)
                {
                    Rect rectRecharge = rect;
                    rectRecharge.height = rectBar.height / 12f;
                    rectRecharge.y = rectBar.y - (rectBar.height - rectRecharge.height) / 2f;
                    Widgets.FillableBar(rectRecharge, 1f-rechargeFillPercent, RegenShieldBarTex, EmptyShieldBarTex, false);
                }

                Text.Anchor = TextAnchor.UpperLeft;
            }, true, false, 1f);
            return new GizmoResult(GizmoState.Clear);
        }

        public MutationPsionicShield mutation;

        private static readonly Texture2D FullShieldBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(81f / 255, 13f / 255, 255f / 255));

        private static readonly Texture2D RegenShieldBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 1f, 1f));

        private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);
    }


    public class CompPsionicShield : ThingComp
    {
        public MutationPsionicShield mutation;

        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            mutation.ApplyDamage(dinfo, out absorbed);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (mutation.def.health == 0) yield break;

            if (Find.Selector.SingleSelectedThing == mutation.pawn)
            {
                yield return new GizmoPsionicShieldStatus
                {
                    mutation = mutation
                };
            }

            yield break;
        }

    }

    public class MutationPsionicShieldDef : MutationDef
    {
        public MutationPsionicShieldDef(){ hediffClass = typeof(MutationPsionicShield); }

        public float health;
        public float regenratedPerSecond;

        public int regenerationDelayTicks;

        public RadiologyEffectSpawnerDef effectAbsorbed;
        public RadiologyEffectSpawnerDef effectBroken;
        public RadiologyEffectSpawnerDef effectRestored;

        public List<DamageDef> protectsAgainst;
        public List<DamageDef> uselessAgainst;

    }

    public class MutationPsionicShield : Mutation<MutationPsionicShieldDef>
    {
        public override ThingComp[] GetComps()
        {
            return new ThingComp[] { new CompPsionicShield() { mutation = this } };
        }

        public void ApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            absorbed = false;

            if (def.protectsAgainst != null && !def.protectsAgainst.Contains(dinfo.Def)) return;
            if (def.uselessAgainst != null && def.uselessAgainst.Contains(dinfo.Def)) return;

            regenerationDelay = def.regenerationDelayTicks;

            if (health == 0 && def.health != 0) return;

            if (dinfo.Amount <= health || def.health == 0)
            {
                health -= dinfo.Amount;
                absorbed = true;

                RadiologyEffectSpawnerDef.Spawn(def.effectAbsorbed, pawn, dinfo.Angle + 180);
            }
            else
            {
                dinfo.SetAmount(dinfo.Amount - health);
                health = 0;
            }

            if (health == 0 && def.health != 0)
            {
                RadiologyEffectSpawnerDef.Spawn(def.effectBroken, pawn, dinfo.Angle + 180);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref health, "health");
            Scribe_Values.Look(ref regenerationDelay, "regenerationDelay");
        }

        public override void Tick()
        {
            base.Tick();

            if (regenerationDelay > 0)
            {
                regenerationDelay--;
            }
            else
            {
                if (health == 0 && def.health!=0 && def.regenratedPerSecond != 0)
                {
                    RadiologyEffectSpawnerDef.Spawn(def.effectRestored, pawn);
                }

                health += def.regenratedPerSecond / 60;
                if (health > def.health) health = def.health;
            }
        }

        public float health = 0;
        public int regenerationDelay = 0;
    }

}
