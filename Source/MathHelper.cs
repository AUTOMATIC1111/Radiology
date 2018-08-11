using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiology
{
    public class MathHelper
    {
        public static bool IsSameSign(int a, int b)
        {
            return a > 0 && b > 0 || a < 0 && b < 0;
        }
        public static bool IsSameSign(float a, float b)
        {
            return a > 0 && b > 0 || a < 0 && b < 0;
        }
        public static bool IsBetween(int c, int a, int b)
        {
            return a <= c && c <= b;
        }
    }
}
