using SortAlgorithm;
using System;
using System.Collections;
using System.Linq;

namespace SortRunner
{
    class Program
    {
        //private static readonly int[] sample = new[] { 2, 5, 7, 0, 1, 0 };
        private static int[] sample = Enumerable.Range(0, 100).Sample(100).ToArray();
        private static int[] keep = new int[sample.Length];

        static void Main(string[] args)
        {
            ResetArray(ref sample, ref keep);

            // Bubble Sort
            var bubbleSort = new BubbleSort<int>();
            Run(bubbleSort, nameof(BubbleSort<int>));
        }

        static void Run(ISort<int> sort, string sortKind)
        {
            var after = sort.Sort(sample);

            Console.WriteLine($@"{nameof(sortKind)} : {sortKind}, {nameof(sort.SortStatics.ArraySize)} : {sort.SortStatics.ArraySize}, {nameof(sort.SortStatics.IndexAccessCount)} : {sort.SortStatics.IndexAccessCount}, {nameof(sort.SortStatics.SwapCount)} : {sort.SortStatics.SwapCount}
Before : {keep.ToJoinedString(" ")}
After  : {after.ToJoinedString(" ")}");

            ResetArray(ref keep, ref sample);
        }

        static void ResetArray<T>(ref T[] source, ref T[] target) where T : IComparable
        {
            source.CopyTo(target, 0);
        }
    }
}
