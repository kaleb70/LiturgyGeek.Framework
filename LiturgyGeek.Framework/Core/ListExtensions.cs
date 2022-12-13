using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Core
{
    public static class ListExtensions
    {
        public static List<T> Clone<T>(this List<T> list) where T : ICloneable<T>
            => list.Select(e => e.Clone()).ToList();
    }
}
