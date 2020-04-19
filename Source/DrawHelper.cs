    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Radiology
{
    static class DrawHelper
    {
        static MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        static Matrix4x4 matrix;

        public static void DrawMesh(Matrix4x4 matrix, Material matSingle, Color color)
        {
            propertyBlock.SetColor(ShaderPropertyIDs.Color, color);
            Graphics.DrawMesh(MeshPool.plane10, matrix, matSingle, -1, null, 0, propertyBlock);
        }

        public static void DrawMesh(Vector3 vec, int v, Vector3 scale, Material matSingle, Color color)
        {
            matrix.SetTRS(vec, Quaternion.AngleAxis(0, Vector3.up), scale);
            DrawMesh(matrix, matSingle, color);
        }

        public static Color Mix(Color a, Color b, float amountOfA)
        {
            amountOfA = Mathf.Clamp01(amountOfA);
            float amountOfB = 1f - amountOfA;
            return new Color(a.r * amountOfA + b.r * amountOfB, a.g * amountOfA + b.g * amountOfB, a.b * amountOfA + b.b * amountOfB);
        }
    }
}
