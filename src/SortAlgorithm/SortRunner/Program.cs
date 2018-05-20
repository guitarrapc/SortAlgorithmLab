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

            // Shaker Sort
            var shakerSort = new ShakerSort<int>();
            Run(shakerSort, ref sample, nameof(ShakerSort<int>));
            Run(shakerSort, ref sample2, nameof(ShakerSort<int>));
            //Run(shakerSort, ref sample3, nameof(ShakerSort<int>));

            // Selection Sort
            var selectionSort = new SelectionSort<int>();
            Run(selectionSort, ref sample, nameof(SelectionSort<int>));
            //Run(selectionSort, ref sample2, nameof(SelectionSort<int>));

            // Insert Sort
            var insertSort = new InsertSort<int>();
            Run(insertSort, ref sample, nameof(InsertSort<int>));
            Run(insertSort, ref sample2, nameof(InsertSort<int>));
            //Run(insertSort, ref sample3, nameof(InsertSort<int>));

            // Shell Sort
            var shellSort = new ShellSort<int>();
            Run(shellSort, ref sample, nameof(ShellSort<int>));
            Run(shellSort, ref sample2, nameof(ShellSort<int>));
            //Run(shellSort, ref sample3, nameof(ShellSort<int>));
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
