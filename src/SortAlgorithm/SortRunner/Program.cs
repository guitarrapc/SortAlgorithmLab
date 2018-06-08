using SortAlgorithm;
using SortAlgorithm.Logics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SortRunner
{
    class Program
    {
        //private static readonly int[] sample = new[] { 2, 5, 7, 0, 1, 0 };
        private static readonly int[] sample = Enumerable.Range(0, 100).Sample(100).ToArray();
        private static readonly int[] sample2 = Enumerable.Range(0, 1000).Sample(1000).ToArray();
        private static readonly int[] sample3 = Enumerable.Range(0, 10000).Sample(10000).ToArray();
        private static readonly int[] sample4 = Enumerable.Range(0, 100000).Sample(100000).ToArray();
        private static int i = 0;
        private static readonly KeyValuePair<int, string>[] dicSample = sample.Select(x => new KeyValuePair<int, string>(x, $"{x / 25}{((char)(65 + (x % 26)))}{i++}")).ToArray();
        private static readonly KeyValuePair<int, string>[] dicSample2 = sample2.Select(x => new KeyValuePair<int, string>(x, $"{x / 25}{((char)(65 + (x % 26)))}{i++}")).ToArray();
        private static readonly KeyValuePair<int, string>[] dicSample3 = sample3.Select(x => new KeyValuePair<int, string>(x, $"{x / 25}{((char)(65 + (x % 26)))}{i++}")).ToArray();

        static void Main(string[] args)
        {
            foreach (var item in new[] { sample, sample2, sample3 })
            {
                // Init
                Runner.Init(item);

                // -- Exchange -- //

                // Bubble Sort
                Runner.Run(new BubbleSort<int>(), item);

                // Cocktail Shaker Sort
                Runner.Run(new CocktailShakerSort<int>(), item);
                Runner.Run(new CocktailShakerSort2<int>(), item);

                // Comb Sort
                Runner.Run(new CombSort<int>(), item);

                // Cycle Sort
                Runner.Run(new CycleSort<int>(), item);

                // Gnome Sort
                Runner.Run(new GnomeSort<int>(), item);
                Runner.Run(new GnomeSort1<int>(), item);
                Runner.Run(new GnomeSort2<int>(), item);
                Runner.Run(new GnomeSort3<int>(), item);

                // Quick Sort Median3
                Runner.Run(new QuickSortMedian3<int>(), item);

                // Quick Sort Median9
                Runner.Run(new QuickSortMedian9<int>(), item);

                // Dual Pivot QuickSort
                Runner.Run(new QuickDualPivotSort<int>(), item);

                // QuickSort Median3 (Quick + Insert)
                Runner.Run(new QuickSortMedian3Insert<int>(), item);

                // QuickSort Median9 (Quick + Insert)
                Runner.Run(new QuickSortMedian9Insert<int>(), item);

                // Dual Pivot Quick Sort (Quick + Insert)
                Runner.Run(new QuickDualPivotSortInsert<int>(), item);

                // QuickSort Median3 (Quick + BinaryInsert)
                Runner.Run(new QuickSortMedian3BinaryInsert<int>(), item);

                // QuickSort Median9 (Quick + BinaryInsert)
                Runner.Run(new QuickSortMedian9BinaryInsert<int>(), item);

                // Dual Pivot Quick Sort (Quick + BinaryInsert)
                Runner.Run(new QuickDualPivotSortBinaryInsert<int>(), item);

                // -- Selection -- //

                // Selection Sort
                Runner.Run(new SelectionSort<int>(), item);

                // Heap Sort
                Runner.Run(new HeapSort<int>(), item);

                // -- Insertion -- //

                // Insert Sort
                Runner.Run(new InsertSort<int>(), item);

                // Binary Insert Sort
                Runner.Run(new BinaryInsertSort<int>(), item);

                // Shell Sort
                Runner.Run(new ShellSort<int>(), item);

                // -- Merge -- //

                // Merge Sort
                Runner.Run(new MergeSort<int>(), item);
                Runner.Run(new MergeSort2<int>(), item);

                // -- Distribution -- //

                // Bucket Sort
                var bucketSort = new BucketSort<int>();
                Runner.Run(bucketSort, array => bucketSort.Sort(array), item);

                // Radix Sort(LSD)
                var radix10Sort = new RadixLSD10Sort<int>();
                Runner.Run(radix10Sort, array => radix10Sort.Sort(array), item);

                var radix4Sort = new RadixLSD4Sort<int>();
                Runner.Run(radix4Sort, array => radix4Sort.Sort(array), item);

                // Counting Sort
                var countingSort = new CountingSort<int>();
                Runner.Run(countingSort, array => countingSort.Sort(array), item);

                // -- Hybrid -- //

                // IntroSort Median3 (Quick + Heap + Insert)
                Runner.Run(new IntroSortMedian3<int>(), item);

                // IntroSort Median9 (Quick + Heap + Insert)
                Runner.Run(new IntroSortMedian9<int>(), item);

            }

            foreach (var dicItem in new[] { dicSample, dicSample2, dicSample3 })
            {
                // Init
                Runner.Init(dicItem);

                // BucketSort<T>
                Runner.RunBucketTSort(new BucketSortT<KeyValuePair<int, string>>(), x => x.Key, dicItem.Max(x => x.Key), dicItem);
            }
        }
    }

    public static class Runner
    {
        private static int[] validateArray;
        private static KeyValuePair<int, string>[] validateDic;

        public static void Init(int[] array)
        {
            validateArray = array.OrderBy(x => x).ToArray();
        }

        public static void Init(KeyValuePair<int, string>[] array)
        {
            validateDic = array.OrderBy(x => x.Key).ToArray();
        }

        public static void Run(ISort<int> sort, int[] array)
        {
            // prerequites
            var keep = new int[array.Length];
            ResetArray(ref array, ref keep);

            // run sort
            var after = sort.Sort(array);

            // validate
            sort.Statics.IsSorted = after.SequenceEqual(validateArray);

            // result
            var sortResult = sort.Statics.IsSorted ? "" : $@"
Before : {keep.ToJoinedString(" ")}
After  : {after.ToJoinedString(" ")}";
            Console.WriteLine($@"{nameof(sort.Statics.ArraySize)} : {sort.Statics.ArraySize}, {nameof(sort.Statics.IsSorted)} : {sort.Statics.IsSorted}, {nameof(sort.SortType)} : {sort.SortType}, {nameof(sort.Statics.Algorithm)} : {sort.Statics.Algorithm}, {nameof(sort.Statics.IndexAccessCount)} : {sort.Statics.IndexAccessCount}, {nameof(sort.Statics.CompareCount)} : {sort.Statics.CompareCount}, {nameof(sort.Statics.SwapCount)} : {sort.Statics.SwapCount}{sortResult}");

            // reset
            ResetArray(ref keep, ref array);
        }

        public static void Run(ISort<int> sort, Func<int[], int[]> func, int[] array)
        {
            // prerequites
            var keep = new int[array.Length];
            ResetArray(ref array, ref keep);

            // run sort
            var after = func(array);

            // validate
            sort.Statics.IsSorted = after.SequenceEqual(validateArray);

            // result
            var sortResult = sort.Statics.IsSorted ? "" : $@"
Before : {keep.ToJoinedString(" ")}
After  : {after.ToJoinedString(" ")}";
            Console.WriteLine($@"{nameof(sort.Statics.ArraySize)} : {sort.Statics.ArraySize}, {nameof(sort.Statics.IsSorted)} : {sort.Statics.IsSorted}, {nameof(sort.SortType)} : {sort.SortType}, {nameof(sort.Statics.Algorithm)} : {sort.Statics.Algorithm}, {nameof(sort.Statics.IndexAccessCount)} : {sort.Statics.IndexAccessCount}, {nameof(sort.Statics.CompareCount)} : {sort.Statics.CompareCount}, {nameof(sort.Statics.SwapCount)} : {sort.Statics.SwapCount}{sortResult}");

            // reset
            ResetArray(ref keep, ref array);
        }

        public static void RunBucketTSort(BucketSortT<KeyValuePair<int, string>> sort, Func<KeyValuePair<int, string>, int> func, int max, KeyValuePair<int, string>[] array)
        {
            // prerequites
            var keep = new KeyValuePair<int, string>[array.Length];
            ResetArray(ref array, ref keep);

            // run sort
            var after = sort.Sort(array, func, max);

            // validate
            sort.Statics.IsSorted = after.SequenceEqual(validateDic);

            // result
            var sortResult = sort.Statics.IsSorted ? "" : $@"
Before : {keep.ToJoinedString(" ")}
After  : {after.ToJoinedString(" ")}";
            Console.WriteLine($@"{nameof(sort.Statics.ArraySize)} : {sort.Statics.ArraySize}, {nameof(sort.Statics.IsSorted)} : {sort.Statics.IsSorted}, {nameof(sort.SortType)} : {sort.SortType}, {nameof(sort.Statics.Algorithm)} : {sort.Statics.Algorithm}, {nameof(sort.Statics.IndexAccessCount)} : {sort.Statics.IndexAccessCount}, {nameof(sort.Statics.CompareCount)} : {sort.Statics.CompareCount}, {nameof(sort.Statics.SwapCount)} : {sort.Statics.SwapCount}{sortResult}");

            // reset
            ResetArray(ref keep, ref array);
        }

        static void ResetArray<T>(ref T[] source, ref T[] target) where T : IComparable
        {
            source.CopyTo(target, 0);
        }

        static void ResetArray<TKey, TValue>(ref KeyValuePair<TKey, TValue>[] source, ref KeyValuePair<TKey, TValue>[] target)
        {
            source.CopyTo(target, 0);
        }
    }
}
