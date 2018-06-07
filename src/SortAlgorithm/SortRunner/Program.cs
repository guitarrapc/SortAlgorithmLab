﻿using SortAlgorithm;
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
        private static int[] sample = Enumerable.Range(0, 100).Sample(100).ToArray();
        private static int[] sample2 = Enumerable.Range(0, 1000).Sample(1000).ToArray();
        private static int[] sample3 = Enumerable.Range(0, 10000).Sample(10000).ToArray();
        private static int[] sample4 = Enumerable.Range(0, 100000).Sample(100000).ToArray();
        private static int i = 0;
        private static KeyValuePair<int, string>[] dicSample = sample.Select(x => new KeyValuePair<int, string>(x, $"{x / 25}{((char)(65 + (x % 26)))}{i++}")).ToArray();
        private static KeyValuePair<int, string>[] dicSample2 = sample2.Select(x => new KeyValuePair<int, string>(x, $"{x / 25}{((char)(65 + (x % 26)))}{i++}")).ToArray();
        private static KeyValuePair<int, string>[] dicSample3 = sample3.Select(x => new KeyValuePair<int, string>(x, $"{x / 25}{((char)(65 + (x % 26)))}{i++}")).ToArray();

        static void Main(string[] args)
        {
            foreach (var item in new[] { sample, sample2, sample3 })
            {
                // Init
                Runner.Init(item);

                // Bubble Sort
                var bubbleSort = new BubbleSort<int>();
                Runner.Run(bubbleSort, item, nameof(BubbleSort<int>));

                // Selection Sort
                var selectionSort = new SelectionSort<int>();
                Runner.Run(selectionSort, item, nameof(SelectionSort<int>));

                // Insert Sort
                var insertSort = new InsertSort<int>();
                Runner.Run(insertSort, item, nameof(InsertSort<int>));

                // Binary Insert Sort
                var binaryInsertSort = new BinaryInsertSort<int>();
                Runner.Run(binaryInsertSort, item, nameof(BinaryInsertSort<int>));

                // Shaker Sort (Cocktail Sort)
                var shakerSort = new ShakerSort<int>();
                Runner.Run(shakerSort, item, nameof(ShakerSort<int>));
                var shakerSort2 = new ShakerSort2<int>();
                Runner.Run(shakerSort2, item, nameof(ShakerSort2<int>));

                // Comb Sort
                var combSort = new CombSort<int>();
                Runner.Run(combSort, item, nameof(CombSort<int>));

                // Shell Sort
                var shellSort = new ShellSort<int>();
                Runner.Run(shellSort, item, nameof(ShellSort<int>));

                // Gnome Sort
                var gnomeSort = new GnomeSort<int>();
                Runner.Run(gnomeSort, item, nameof(GnomeSort<int>));
                var gnomeSort1 = new GnomeSort1<int>();
                Runner.Run(gnomeSort1, item, nameof(GnomeSort1<int>));
                var gnomeSort2 = new GnomeSort2<int>();
                Runner.Run(gnomeSort2, item, nameof(GnomeSort2<int>));
                var gnomeSort3 = new GnomeSort3<int>();
                Runner.Run(gnomeSort3, item, nameof(GnomeSort3<int>));

                // Quick Sort
                var quickSort = new QuickSort<int>();
                Runner.Run(quickSort, item, nameof(QuickSort<int>));

                // QuickSort (Quick + Insert)
                var quicksortInsert = new QuickSortInsert<int>();
                Runner.Run(quicksortInsert, item, nameof(QuickSortInsert<int>));

                // QuickSort (Quick + BinaryInsert)
                var quicksortBinaryInsert = new QuickSortBinaryInsert<int>();
                Runner.Run(quicksortBinaryInsert, item, nameof(QuickSortBinaryInsert<int>));

                // IntroSort (Quick + Heap + Insert)
                var IntroInsert = new IntroSort<int>();
                Runner.Run(IntroInsert, item, nameof(IntroSort<int>));

                // Merge Sort
                var mergeSort = new MergeSort<int>();
                Runner.Run(mergeSort, item, nameof(MergeSort<int>));

                var mergeSort2 = new MergeSort2<int>();
                Runner.Run(mergeSort2, item, nameof(MergeSort2<int>));

                // Merge Sort
                var heapSort = new HeapSort<int>();
                Runner.Run(heapSort, item, nameof(HeapSort<int>));

                // Bucket Sort
                var bucketSort = new BucketSort<int>();
                Runner.Run(bucketSort, array => bucketSort.Sort(array), item, nameof(BucketSort<int>));

                // Radix Sort(LSD)
                var radix10Sort = new RadixLSD10Sort<int>();
                Runner.Run(radix10Sort, array => radix10Sort.Sort(array), item, nameof(RadixLSD10Sort<int>));

                var radix4Sort = new RadixLSD4Sort<int>();
                Runner.Run(radix4Sort, array => radix4Sort.Sort(array), item, nameof(RadixLSD4Sort<int>));

                // Counting Sort
                var countingSort = new CountingSort<int>();
                Runner.Run(countingSort, array => countingSort.Sort(array), item, nameof(CountingSort<int>));
            }

            foreach (var item in new[] { dicSample, dicSample2, dicSample3 })
            {
                // Init
                Runner.Init(item);

                // BucketSort<T>
                var bucketSortT = new BucketSortT<KeyValuePair<int, string>>();
                Runner.RunBucketTSort(bucketSortT, x => x.Key, item.Max(x => x.Key), item, nameof(BucketSortT<int>));
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

        public static void Run(ISort<int> sort, int[] array, string sortKind)
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
            Console.WriteLine($@"{nameof(sort.Statics.ArraySize)} : {sort.Statics.ArraySize}, {nameof(sort.Statics.IsSorted)} : {sort.Statics.IsSorted}, {nameof(sortKind)} : {sortKind}, {nameof(sort.Statics.IndexAccessCount)} : {sort.Statics.IndexAccessCount}, {nameof(sort.Statics.CompareCount)} : {sort.Statics.CompareCount}, {nameof(sort.Statics.SwapCount)} : {sort.Statics.SwapCount}{sortResult}");

            // reset
            ResetArray(ref keep, ref array);
        }

        public static void Run(ISort<int> sort, Func<int[], int[]> func, int[] array, string sortKind)
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
            Console.WriteLine($@"{nameof(sort.Statics.ArraySize)} : {sort.Statics.ArraySize}, {nameof(sort.Statics.IsSorted)} : {sort.Statics.IsSorted}, {nameof(sortKind)} : {sortKind}, {nameof(sort.Statics.IndexAccessCount)} : {sort.Statics.IndexAccessCount}, {nameof(sort.Statics.CompareCount)} : {sort.Statics.CompareCount}, {nameof(sort.Statics.SwapCount)} : {sort.Statics.SwapCount}{sortResult}");

            // reset
            ResetArray(ref keep, ref array);
        }

        public static void RunBucketTSort(BucketSortT<KeyValuePair<int, string>> sort, Func<KeyValuePair<int, string>, int> func, int max, KeyValuePair<int, string>[] array, string sortKind)
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
            Console.WriteLine($@"{nameof(sort.Statics.ArraySize)} : {sort.Statics.ArraySize}, {nameof(sort.Statics.IsSorted)} : {sort.Statics.IsSorted}, {nameof(sortKind)} : {sortKind}, {nameof(sort.Statics.IndexAccessCount)} : {sort.Statics.IndexAccessCount}, {nameof(sort.Statics.CompareCount)} : {sort.Statics.CompareCount}, {nameof(sort.Statics.SwapCount)} : {sort.Statics.SwapCount}{sortResult}");

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
