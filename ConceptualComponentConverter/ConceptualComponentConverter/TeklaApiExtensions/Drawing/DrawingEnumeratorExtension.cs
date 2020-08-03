﻿using System.Collections.Generic;
using Tekla.Structures.Drawing;

namespace TeklaOpenAPIExtension
{
    public static class DrawingEnumeratorExtension
    {
        /// <summary>Add items from the enumerator to the System.Collections.Generic.List</summary>
        public static List<Drawing> ToList(this DrawingEnumerator enumerator)
        {
            var output = new List<Drawing>(enumerator.GetSize());

            while (enumerator.MoveNext())
            {
                output.Add(enumerator.Current);
            }

            return output;
        }

        /// <summary>Add items from the enumerator to the System.Collections.Generic.List. if (enumerator.Current is T t) output.Add(t);</summary>
        public static List<T> ToList<T>(this DrawingEnumerator enumerator) where T : Drawing
        {
            var output = new List<T>(enumerator.GetSize());

            while (enumerator.MoveNext())
            {
                if (enumerator.Current is T t)
                    output.Add(t);
            }

            return output;
        }
    }
}
