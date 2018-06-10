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
        private static readonly IInputSample<int> sample = new InputSample<int>() { InputType = InputType.Random, Samples = Enumerable.Range(0, 100).Sample(100).ToArray() };
        private static readonly IInputSample<int> sample2 = new InputSample<int>() { InputType = InputType.Random, Samples = Enumerable.Range(0, 1000).Sample(1000).ToArray() };
        private static readonly IInputSample<int> sample3 = new InputSample<int>() { InputType = InputType.Random, Samples = Enumerable.Range(0, 10000).Sample(10000).ToArray() };

        private static readonly IInputSample<int> npsample = new InputSample<int>() { InputType = InputType.MixRandom, Samples = Enumerable.Range(-50, 100).Sample(100).ToArray() };
        private static readonly IInputSample<int> npsample2 = new InputSample<int>() { InputType = InputType.MixRandom, Samples = Enumerable.Range(-500, 1000).Sample(1000).ToArray() };
        private static readonly IInputSample<int> npsample3 = new InputSample<int>() { InputType = InputType.MixRandom, Samples = Enumerable.Range(-5000, 10000).Sample(10000).ToArray() };

        private static readonly IInputSample<int> nsample = new InputSample<int>() { InputType = InputType.NegativeRandom, Samples = Enumerable.Range(-100, 100).Sample(100).ToArray() };
        private static readonly IInputSample<int> nsample2 = new InputSample<int>() { InputType = InputType.NegativeRandom, Samples = Enumerable.Range(-1000, 1000).Sample(1000).ToArray() };
        private static readonly IInputSample<int> nsample3 = new InputSample<int>() { InputType = InputType.NegativeRandom, Samples = Enumerable.Range(-10000, 10000).Sample(10000).ToArray() };

        private static readonly IInputSample<int> reversedsample = new InputSample<int>() { InputType = InputType.Reversed, Samples = Enumerable.Range(0, 100).Reverse().ToArray() };
        private static readonly IInputSample<int> reversedsample2 = new InputSample<int>() { InputType = InputType.Reversed, Samples = Enumerable.Range(0, 1000).Reverse().ToArray() };
        private static readonly IInputSample<int> reversedsample3 = new InputSample<int>() { InputType = InputType.Reversed, Samples = Enumerable.Range(0, 10000).Reverse().ToArray() };

        private static readonly IInputSample<int> mountainsample = new InputSample<int>() { InputType = InputType.Mountain, Samples = Enumerable.Range(0, 50).Concat(Enumerable.Range(0, 50).Reverse()).ToArray() };
        private static readonly IInputSample<int> mountainsample2 = new InputSample<int>() { InputType = InputType.Mountain, Samples = Enumerable.Range(0, 500).Concat(Enumerable.Range(0, 500).Reverse()).ToArray() };
        private static readonly IInputSample<int> mountainsample3 = new InputSample<int>() { InputType = InputType.Mountain, Samples = Enumerable.Range(0, 5000).Concat(Enumerable.Range(0, 5000).Reverse()).ToArray() };

        private static readonly IInputSample<int> nearlysample = new InputSample<int>() { InputType = InputType.NearlySorted, Samples = Enumerable.Range(0, 90).Concat(Enumerable.Range(0, 100).Sample(10)).ToArray() };
        private static readonly IInputSample<int> nearlysample2 = new InputSample<int>() { InputType = InputType.NearlySorted, Samples = Enumerable.Range(0, 990).Concat(Enumerable.Range(0, 1000).Sample(10)).ToArray() };
        private static readonly IInputSample<int> nearlysample3 = new InputSample<int>() { InputType = InputType.NearlySorted, Samples = Enumerable.Range(0, 9990).Concat(Enumerable.Range(0, 10000).Sample(10)).ToArray() };

        private static readonly IInputSample<int> sortedsample = new InputSample<int>() { InputType = InputType.Sorted, Samples = Enumerable.Range(0, 100).ToArray() };
        private static readonly IInputSample<int> sortedsample2 = new InputSample<int>() { InputType = InputType.Sorted, Samples = Enumerable.Range(0, 1000).ToArray() };
        private static readonly IInputSample<int> sortedsample3 = new InputSample<int>() { InputType = InputType.Sorted, Samples = Enumerable.Range(0, 10000).ToArray() };

        private static readonly int[] randomeSample = Enumerable.Range(0, 100).Sample(10).ToArray();
        private static readonly int[] randomeSample2 = Enumerable.Range(0, 1000).Sample(10).ToArray();
        private static readonly int[] randomeSample3 = Enumerable.Range(0, 10000).Sample(10).ToArray();
        private static readonly IInputSample<int> samevaluessample = new InputSample<int>() { InputType = InputType.SameValues, Samples = randomeSample.SelectMany(x => Enumerable.Repeat(x, 10)).Sample(100).ToArray() };
        private static readonly IInputSample<int> samevaluessample2 = new InputSample<int>() { InputType = InputType.SameValues, Samples = randomeSample2.SelectMany(x => Enumerable.Repeat(x, 100)).Sample(1000).ToArray() };
        private static readonly IInputSample<int> samevaluessample3 = new InputSample<int>() { InputType = InputType.SameValues, Samples = randomeSample3.SelectMany(x => Enumerable.Repeat(x, 1000)).Sample(10000).ToArray() };

        private static int i = 0;
        private static readonly IInputSample<int> dicSample = new InputSample<int>() { InputType = InputType.DictionaryRamdom, DictionarySamples = sample.Samples.Select(x => new KeyValuePair<int, string>(x, $"{x / 25}{((char)(65 + (x % 26)))}{i++}")).ToArray() };
        private static readonly IInputSample<int> dicSample2 = new InputSample<int>() { InputType = InputType.DictionaryRamdom, DictionarySamples = sample2.Samples.Select(x => new KeyValuePair<int, string>(x, $"{x / 25}{((char)(65 + (x % 26)))}{i++}")).ToArray() };
        private static readonly IInputSample<int> dicSample3 = new InputSample<int>() { InputType = InputType.DictionaryRamdom, DictionarySamples = sample3.Samples.Select(x => new KeyValuePair<int, string>(x, $"{x / 25}{((char)(65 + (x % 26)))}{i++}")).ToArray() };

        static void Main(string[] args)
        {
            var runner = new Runner();
            runner.Run(new[] { sample, sample2, sample3 });
            runner.Run(new[] { npsample, npsample2, npsample3 });
            runner.Run(new[] { nsample, nsample2, nsample3 });
            runner.Run(new[] { reversedsample, reversedsample2, reversedsample3 });
            runner.Run(new[] { mountainsample, mountainsample2, mountainsample3 });
            runner.Run(new[] { nearlysample, nearlysample2, nearlysample3 });
            runner.Run(new[] { sortedsample, sortedsample2, sortedsample3 });
            runner.Run(new[] { samevaluessample, samevaluessample2, samevaluessample3 });
            runner.Run(new[] { dicSample, dicSample2, dicSample3 });

            Console.WriteLine(runner.MarkDownOutputList.First().Header);
            foreach (var item in runner.MarkDownOutputList.Select(x => x.Item))
            {
                Console.WriteLine(item);
            }
        }
    }

    public class Runner
    {
        private static int[] validateArray;
        private static KeyValuePair<int, string>[] validateDic;
        public List<IOutput> MarkDownOutputList;

        public Runner()
        {
            MarkDownOutputList = new List<IOutput>();
        }

        public void Run(IInputSample<int>[] items)
        {
            foreach (var item in items)
            {
                if (item.Samples != null)
                {
                    Init(item.Samples);

                    // -- Exchange -- //

                    // Bubble Sort
                    RunSort(new BubbleSort<int>(), item);

                    // OddEven Sort
                    RunSort(new OddEvenSort<int>(), item);

                    // Cocktail Shaker Sort
                    RunSort(new CocktailShakerSort<int>(), item);
                    RunSort(new CocktailShakerSort2<int>(), item);

                    // Comb Sort
                    RunSort(new CombSort<int>(), item);

                    // Cycle Sort
                    RunSort(new CycleSort<int>(), item);

                    // Gnome Sort
                    RunSort(new GnomeSort<int>(), item);
                    RunSort(new GnomeSort1<int>(), item);
                    RunSort(new GnomeSort2<int>(), item);
                    RunSort(new GnomeSort3<int>(), item);

                    // -- Selection -- //

                    // Selection Sort
                    RunSort(new SelectionSort<int>(), item);

                    // Heap Sort
                    RunSort(new HeapSort<int>(), item);

                    // -- Insertion -- //

                    // Insert Sort
                    RunSort(new InsertSort<int>(), item);

                    // Binary Insert Sort
                    RunSort(new BinaryInsertSort<int>(), item);

                    // Shell Sort
                    RunSort(new ShellSort<int>(), item);

                    // Binary Tree Sort
                    RunSort(new BinaryTreeSort<int>(), item);

                    // -- Partitionig -- //

                    // Quick Sort Median3
                    RunSort(new QuickSortMedian3<int>(), item);

                    // Quick Sort Median9
                    RunSort(new QuickSortMedian9<int>(), item);

                    // Dual Pivot QuickSort
                    RunSort(new QuickDualPivotSort<int>(), item);

                    // QuickSort Median3 (Quick + Insert)
                    RunSort(new QuickSortMedian3Insert<int>(), item);

                    // QuickSort Median9 (Quick + Insert)
                    RunSort(new QuickSortMedian9Insert<int>(), item);

                    // Dual Pivot Quick Sort (Quick + Insert)
                    RunSort(new QuickDualPivotSortInsert<int>(), item);

                    // QuickSort Median3 (Quick + BinaryInsert)
                    RunSort(new QuickSortMedian3BinaryInsert<int>(), item);

                    // QuickSort Median9 (Quick + BinaryInsert)
                    RunSort(new QuickSortMedian9BinaryInsert<int>(), item);

                    // Dual Pivot Quick Sort (Quick + BinaryInsert)
                    RunSort(new QuickDualPivotSortBinaryInsert<int>(), item);

                    // -- Merge -- //

                    // Merge Sort
                    RunSort(new MergeSort<int>(), item);
                    RunSort(new MergeSort2<int>(), item);

                    // Shift Sort
                    RunSort(new ShiftSort<int>(), item);

                    // -- Distribution -- //

                    // Bucket Sort
                    var bucketSort = new BucketSort<int>();
                    RunSort(bucketSort, array => bucketSort.Sort(array), item);

                    // Radix Sort(LSD)
                    var radix10Sort = new RadixLSD10Sort<int>();
                    RunSort(radix10Sort, array => radix10Sort.Sort(array), item);

                    var radix4Sort = new RadixLSD4Sort<int>();
                    RunSort(radix4Sort, array => radix4Sort.Sort(array), item);

                    // Counting Sort
                    var countingSort = new CountingSort<int>();
                    RunSort(countingSort, array => countingSort.Sort(array), item);

                    // -- Hybrid -- //

                    // IntroSort Median3 (Quick + Heap + Insert)
                    //RunSort(new IntroSortMedian3<int>(), item);

                    // IntroSort Median9 (Quick + Heap + Insert)
                    RunSort(new IntroSortMedian9<int>(), item);
                }

                // BucketSort<T>
                if (item.Samples == null && item.DictionarySamples != null)
                {
                    Init(item.DictionarySamples);

                    // -- Distribution -- //

                    RunBucketTSort(new BucketSortT<KeyValuePair<int, string>>(), x => x.Key, item.DictionarySamples.Max(x => x.Key), item);
                }
            }
        }

        private void Init(int[] input)
        {
            validateArray = input.OrderBy(x => x).ToArray();
        }

        private void Init(KeyValuePair<int, string>[] array)
        {
            validateDic = array.OrderBy(x => x.Key).ToArray();
        }

        private void RunSort(ISort<int> sort, IInputSample<int> input)
        {
            var array = input.Samples;

            // prerequites
            var keep = new int[array.Length];
            ResetArray(ref array, ref keep);

            // run sort
            var after = sort.Sort(array);

            // validate
            sort.Statics.IsSorted = after.SequenceEqual(validateArray);

            // result
            MarkDownOutputList.Add(new MarkdownOutput(sort.Statics, input.InputType));

            // Console Output
            var sortResult = sort.Statics.IsSorted ? "Correct" : $@"
Before : {keep.ToJoinedString(" ")}
After  : {after.ToJoinedString(" ")}";
            var console = new ConsoleOutput(sort.Statics, sortResult, input.InputType);
            Console.WriteLine(console.ToString());

            // reset
            ResetArray(ref keep, ref array);
        }

        private void RunSort(ISort<int> sort, Func<int[], int[]> func, IInputSample<int> input)
        {
            var array = input.Samples;

            // prerequites
            var keep = new int[array.Length];
            ResetArray(ref array, ref keep);

            // run sort
            var after = func(array);

            // validate
            sort.Statics.IsSorted = after.SequenceEqual(validateArray);

            // result
            MarkDownOutputList.Add(new MarkdownOutput(sort.Statics, input.InputType));

            // Console Output
            var sortResult = sort.Statics.IsSorted ? "Correct" : $@"
Before : {keep.ToJoinedString(" ")}
After  : {after.ToJoinedString(" ")}";
            var console = new ConsoleOutput(sort.Statics, sortResult, input.InputType);
            Console.WriteLine(console.ToString());

            // reset
            ResetArray(ref keep, ref array);
        }

        private void RunBucketTSort(BucketSortT<KeyValuePair<int, string>> sort, Func<KeyValuePair<int, string>, int> func, int max, IInputSample<int> input)
        {
            var array = input.DictionarySamples;

            // prerequites
            var keep = new KeyValuePair<int, string>[array.Length];
            ResetArray(ref array, ref keep);

            // run sort
            var after = sort.Sort(array, func);

            // validate
            sort.Statics.IsSorted = after.SequenceEqual(validateDic);

            // result
            MarkDownOutputList.Add(new MarkdownOutput(sort.Statics, input.InputType));

            // Console Output
            var sortResult = sort.Statics.IsSorted ? "Correct" : $@"
Before : {keep.ToJoinedString(" ")}
After  : {after.ToJoinedString(" ")}";
            var console = new ConsoleOutput(sort.Statics, sortResult, input.InputType);
            Console.WriteLine(console.ToString());

            // reset
            ResetArray(ref keep, ref array);
        }

        private void ResetArray<T>(ref T[] source, ref T[] target) where T : IComparable
        {
            source.CopyTo(target, 0);
        }

        private void ResetArray<TKey, TValue>(ref KeyValuePair<TKey, TValue>[] source, ref KeyValuePair<TKey, TValue>[] target)
        {
            source.CopyTo(target, 0);
        }
    }
}
