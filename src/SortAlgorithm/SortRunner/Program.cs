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
        private static int[] sample = Enumerable.Range(0, 100).Sample(100).ToArray();
        private static int[] sample2 = Enumerable.Range(0, 200).Sample(200).ToArray();
        private static int[] sample3 = Enumerable.Range(0, 1280).Sample(1280).ToArray();
        private static int i = 0;
        private static KeyValuePair<int, string>[] dicSample = sample.Select(x => new KeyValuePair<int, string>(x, $"{x / 25}{((char)(65 + (x % 26)))}{i++}")).ToArray();
        private static KeyValuePair<int, string>[] dicSample2 = sample2.Select(x => new KeyValuePair<int, string>(x, $"{x / 25}{((char)(65 + (x % 26)))}{i++}")).ToArray();
        private static KeyValuePair<int, string>[] dicSample3 = sample3.Select(x => new KeyValuePair<int, string>(x, $"{x / 25}{((char)(65 + (x % 26)))}{i++}")).ToArray();

        static void Main(string[] args)
        {
            foreach (var item in new[] { sample, sample2 })
            {
                // Init
                Runner.Init(item);
                Runner.Init(dicSample);

                // Bubble Sort
                var bubbleSort = new BubbleSort<int>();
                Runner.Run(bubbleSort, item, nameof(BubbleSort<int>));

                // Selection Sort
                var selectionSort = new SelectionSort<int>();
                Runner.Run(selectionSort, item, nameof(SelectionSort<int>));

                // Insert Sort
                var insertSort = new InsertSort<int>();
                Runner.Run(insertSort, item, nameof(InsertSort<int>));

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
                var gnomeSort2 = new GnomeSort2<int>();
                Runner.Run(gnomeSort2, item, nameof(GnomeSort2<int>));
                var gnomeSort3 = new GnomeSort3<int>();
                Runner.Run(gnomeSort3, item, nameof(GnomeSort3<int>));

                // Gnome Sort Optimized
                var gnomeSortOpmized = new GnomeSortOptimized<int>();
                Runner.Run(gnomeSortOpmized, item, nameof(GnomeSortOptimized<int>));

                // Quick Sort
                var quickSort = new QuickSort<int>();
                Runner.Run(quickSort, item, nameof(QuickSort<int>));

                // IntroSort (Quick + Insert)
                var introSort = new IntroSortQuickInsert<int>();
                Runner.Run(introSort, item, nameof(IntroSortQuickInsert<int>));

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

                // Radix Sort
                var radix10Sort = new Radix10Sort<int>();
                Runner.Run(radix10Sort, array => radix10Sort.Sort(array), item, nameof(Radix10Sort<int>));

                var radix4Sort = new Radix4Sort<int>();
                Runner.Run(radix4Sort, array => radix4Sort.Sort(array), item, nameof(Radix4Sort<int>));

                var radix2Sort = new Radix2Sort<int>();
                Runner.Run(radix2Sort, array => radix2Sort.Sort(array), item, nameof(Radix2Sort<int>));

                // Counting Sort
                var countingSort = new CountingSort<int>();
                Runner.Run(countingSort, array => countingSort.Sort(array), item, nameof(CountingSort<int>));
            }

            // BucketSort<T>
            var bucketSortT = new BucketSortT<KeyValuePair<int, string>>();
            Runner.RunBucketTSort(bucketSortT, item => item.Key, dicSample.Max(x => x.Key), dicSample, nameof(BucketSortT<int>));
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
            sort.SortStatics.IsSorted = after.SequenceEqual(validateArray);

            // result
            var sortResult = sort.SortStatics.IsSorted ? "" : $@"Before : {keep.ToJoinedString(" ")}
After  : {after.ToJoinedString(" ")}";
            Console.WriteLine($@"{nameof(sortKind)} : {sortKind}, {nameof(sort.SortStatics.IsSorted)} : {sort.SortStatics.IsSorted}, {nameof(sort.SortStatics.ArraySize)} : {sort.SortStatics.ArraySize}, {nameof(sort.SortStatics.IndexAccessCount)} : {sort.SortStatics.IndexAccessCount}, {nameof(sort.SortStatics.CompareCount)} : {sort.SortStatics.CompareCount}, {nameof(sort.SortStatics.SwapCount)} : {sort.SortStatics.SwapCount}
{sortResult}");

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
            sort.SortStatics.IsSorted = after.SequenceEqual(validateArray);

            // result
            var sortResult = sort.SortStatics.IsSorted ? "" : $@"Before : {keep.ToJoinedString(" ")}
After  : {after.ToJoinedString(" ")}";
            Console.WriteLine($@"{nameof(sortKind)} : {sortKind}, {nameof(sort.SortStatics.IsSorted)} : {sort.SortStatics.IsSorted}, {nameof(sort.SortStatics.ArraySize)} : {sort.SortStatics.ArraySize}, {nameof(sort.SortStatics.IndexAccessCount)} : {sort.SortStatics.IndexAccessCount}, {nameof(sort.SortStatics.CompareCount)} : {sort.SortStatics.CompareCount}, {nameof(sort.SortStatics.SwapCount)} : {sort.SortStatics.SwapCount}
{sortResult}");

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
            sort.SortStatics.IsSorted = after.SequenceEqual(validateDic);

            // result
            var sortResult = sort.SortStatics.IsSorted ? "" : $@"Before : {keep.ToJoinedString(" ")}
After  : {after.ToJoinedString(" ")}";
            Console.WriteLine($@"{nameof(sortKind)} : {sortKind}, {nameof(sort.SortStatics.IsSorted)} : {sort.SortStatics.IsSorted}, {nameof(sort.SortStatics.ArraySize)} : {sort.SortStatics.ArraySize}, {nameof(sort.SortStatics.IndexAccessCount)} : {sort.SortStatics.IndexAccessCount}, {nameof(sort.SortStatics.CompareCount)} : {sort.SortStatics.CompareCount}, {nameof(sort.SortStatics.SwapCount)} : {sort.SortStatics.SwapCount}
{sortResult}");

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
