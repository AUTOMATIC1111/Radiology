using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Radiology
{
    [StaticConstructorOnStartup]
    public class Graphic_Mote_Random : Graphic_Random
    {
        // Token: 0x17000D04 RID: 3332
        // (get) Token: 0x060050EF RID: 20719 RVA: 0x002A4598 File Offset: 0x002A2998
        protected virtual bool ForcePropertyBlock
        {
            get
            {
                return false;
            }
        }

        // Token: 0x060050F0 RID: 20720 RVA: 0x002A45AE File Offset: 0x002A29AE
        public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
        {
            this.DrawMoteInternal(loc, rot, thingDef, thing, 0);
        }

        // Token: 0x060050F1 RID: 20721 RVA: 0x002A45C0 File Offset: 0x002A29C0
        public void DrawMoteInternal(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, int layer)
        {
            Mote mote = (Mote)thing;
            float alpha = mote.Alpha;
            if (alpha > 0f)
            {
                Color color = base.Color * mote.instanceColor;
                color.a *= alpha;
                Vector3 exactScale = mote.exactScale;
                exactScale.x *= this.data.drawSize.x;
                exactScale.z *= this.data.drawSize.y;
                Matrix4x4 matrix = default(Matrix4x4);
                matrix.SetTRS(mote.DrawPos, Quaternion.AngleAxis(mote.exactRotation, Vector3.up), exactScale);
                Material matSingle = MaterialFor(thing);

                if (!this.ForcePropertyBlock && color.IndistinguishableFrom(matSingle.color))
                {
                    Graphics.DrawMesh(MeshPool.plane10, matrix, matSingle, layer, null, 0);
                }
                else
                {
                    Graphic_Mote_Random.propertyBlock.SetColor(ShaderPropertyIDs.Color, color);
                    Graphics.DrawMesh(MeshPool.plane10, matrix, matSingle, layer, null, 0, Graphic_Mote_Random.propertyBlock);
                }
            }
        }
        public Material MaterialFor(Thing thing)
        {
            if (thing == null) return MatSingle;

            return subGraphics[(thing.thingIDNumber < 0 ? -thing.thingIDNumber : thing.thingIDNumber) % subGraphics.Length].MatSingle;
        }


        // Token: 0x060050F2 RID: 20722 RVA: 0x002A46DC File Offset: 0x002A2ADC
        public override string ToString()
        {
            return string.Concat(new object[]
            {
                "Mote(path=",
                this.path,
                ", shader=",
                base.Shader,
                ", color=",
                this.color,
                ", colorTwo=unsupported)"
            });
        }

        // Token: 0x04003608 RID: 13832
        protected static MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
    }
}
