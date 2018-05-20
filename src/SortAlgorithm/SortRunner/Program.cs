using SortAlgorithm;
using SortAlgorithm.Logics;
using System;
using System.Collections;
using System.Linq;

namespace SortRunner
{
    class Program
    {
        //private static readonly int[] sample = new[] { 2, 5, 7, 0, 1, 0 };
        private static int[] sample = Enumerable.Range(0, 100).Sample(100).ToArray();
        private static int[] sample2 = Enumerable.Range(0, 200).Sample(200).ToArray();
        private static int[] sample3 = Enumerable.Range(0, 1280).Sample(1280).ToArray();

        static void Main(string[] args)
        {
            // Bubble Sort
            var bubbleSort = new BubbleSort<int>();
            Run(bubbleSort, ref sample, nameof(BubbleSort<int>));
            Run(bubbleSort, ref sample2, nameof(BubbleSort<int>));
            //Run(bubbleSort, ref sample3, nameof(BubbleSort<int>));

            // Selection Sort
            var selectionSort = new SelectionSort<int>();
            Run(selectionSort, ref sample, nameof(SelectionSort<int>));
            Run(selectionSort, ref sample2, nameof(SelectionSort<int>));
            //Run(selectionSort, ref sample3, nameof(SelectionSort<int>));

            // Insert Sort
            var insertSort = new InsertSort<int>();
            Run(insertSort, ref sample, nameof(InsertSort<int>));
            Run(insertSort, ref sample2, nameof(InsertSort<int>));
            //Run(insertSort, ref sample3, nameof(InsertSort<int>));

            // Shaker Sort (Cocktail Sort)
            var shakerSort = new ShakerSort<int>();
            Run(shakerSort, ref sample, nameof(ShakerSort<int>));
            Run(shakerSort, ref sample2, nameof(ShakerSort<int>));
            //Run(shakerSort, ref sample3, nameof(ShakerSort<int>));
            //var shakerSort2 = new ShakerSort2<int>();
            //Run(shakerSort2, ref sample, nameof(ShakerSort2<int>));

            // Comb Sort
            var combSort = new CombSort<int>();
            Run(combSort, ref sample, nameof(CombSort<int>));
            Run(combSort, ref sample2, nameof(CombSort<int>));
            //Run(combSort, ref sample3, nameof(CombSort<int>));

            // Shell Sort
            var shellSort = new ShellSort<int>();
            Run(shellSort, ref sample, nameof(ShellSort<int>));
            Run(shellSort, ref sample2, nameof(ShellSort<int>));
            //Run(shellSort, ref sample3, nameof(ShellSort<int>));

            // Gnome Sort
            var gnomeSort = new GnomeSort<int>();
            Run(gnomeSort, ref sample, nameof(GnomeSort<int>));
            Run(gnomeSort, ref sample2, nameof(GnomeSort<int>));
            //Run(gnomeSort, ref sample3, nameof(GnomeSort<int>));
            var gnomeSort2 = new GnomeSort2<int>();
            Run(gnomeSort2, ref sample, nameof(GnomeSort2<int>));
            var gnomeSort3 = new GnomeSort3<int>();
            Run(gnomeSort3, ref sample, nameof(GnomeSort3<int>));

            // Gnome Sort Optimized
            var gnomeSortOpmized = new GnomeSort<int>();
            Run(gnomeSortOpmized, ref sample, nameof(GnomeSortOptimized<int>));
            Run(gnomeSortOpmized, ref sample2, nameof(GnomeSortOptimized<int>));
            //Run(gnomeSortOpmized, ref sample3, nameof(GnomeSortOptimized<int>));
        }

        static void Run(ISort<int> sort, ref int[] array, string sortKind)
        {
            // prerequites
            var keep = new int[array.Length];
            ResetArray(ref array, ref keep);

            // run sort
            var after = sort.Sort(array);

            // result
            Console.WriteLine($@"{nameof(sortKind)} : {sortKind}, {nameof(sort.SortStatics.ArraySize)} : {sort.SortStatics.ArraySize}, {nameof(sort.SortStatics.IndexAccessCount)} : {sort.SortStatics.IndexAccessCount}, {nameof(sort.SortStatics.CompareCount)} : {sort.SortStatics.CompareCount}, {nameof(sort.SortStatics.SwapCount)} : {sort.SortStatics.SwapCount}
Before : {keep.ToJoinedString(" ")}
After  : {after.ToJoinedString(" ")}");

            // reset
            ResetArray(ref keep, ref array);
        }

        static void ResetArray<T>(ref T[] source, ref T[] target) where T : IComparable
        {
            source.CopyTo(target, 0);
        }
    }
}
