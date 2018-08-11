using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology
{
    public static class Debug
    {
        public static void Log(Object o)
        {
            Verse.Log.Warning("" + o);
        }

        public static string AsText(IEnumerable obj)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("[ ");

            bool first = true;
            foreach (var v in obj)
            {
                if (!first)
                {
                    builder.Append(", ");
                }
                first = false;

                if (v is IEnumerable)
                {
                    builder.Append(AsText(v as IEnumerable));
                }
                else
                {
                    builder.Append(v);
                }
            }
            builder.Append(" ]");

            return builder.ToString();
        }
        public static string AsText<A,B>(Dictionary<A,B> obj)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{ ");

            bool first = true;


            foreach (KeyValuePair<A, B> entry in obj)
            {
                if (!first)
                {
                    builder.Append(", ");
                }
                first = false;

                builder.Append(entry.Key);
                builder.Append(": ");
                if (entry.Value is IEnumerable)
                {
                    builder.Append(AsText(entry.Value as IEnumerable));
                }
                else
                {
                    builder.Append(entry.Value);
                }
            }
            builder.Append(" }");

            return builder.ToString();
        }

        


    }
}
