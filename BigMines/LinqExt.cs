using System;
using System.Collections.Generic;
using VoxelTycoon;
using VoxelTycoon.Cities;

namespace BigMines
{
    public static class LinqExt
    {
        public static void ForEach<T>(this IEnumerable<T> e, Action<T> action)
        {
            foreach (var it in e)
            {
                action(it);
            }
        }

        public static IEnumerable<T> Enumerate<T>(this ImmutableList<T> list)
        {
            return Enumerate(list.Count, i => list[i]);
        }

        public static IEnumerable<CityDemand> Enumerate(this CityDemandCollection list)
        {
            return Enumerate(list.Count, i => list[i]);
        }

        private static IEnumerable<T> Enumerate<T>(int size, Func<int, T> indexer) {
            for (var i = 0; i < size; i++)
            {
                yield return indexer.Invoke(i);
            }
        }
    }
}
