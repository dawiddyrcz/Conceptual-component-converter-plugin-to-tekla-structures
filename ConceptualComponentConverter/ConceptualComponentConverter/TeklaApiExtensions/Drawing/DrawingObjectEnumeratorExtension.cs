﻿using System.Collections.Generic;
using Tekla.Structures.Drawing;

namespace TeklaOpenAPIExtension
{
    public static class DrawingObjectEnumeratorExtension
    {
        /// <summary>Adds items from the enumerator to the System.Collections.Generic.List</summary>
        public static List<DrawingObject> ToList(this DrawingObjectEnumerator enumerator)
        {
            var output = new List<DrawingObject>(enumerator.GetSize());

            while (enumerator.MoveNext())
            {
                output.Add(enumerator.Current);
            }

            return output;
        }

        /// <summary>Adds items from the enumerator to the System.Collections.Generic.List. if (enumerator.Current is T t) output.Add(t);</summary>
        public static List<T> ToList<T>(this DrawingObjectEnumerator enumerator) where T : DrawingObject
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
