using SortLab.Core;
using SortLab.Core.Logics;
using System;
using System.Collections.Generic;
using System.Linq;

var runner = new Runner();
runner.Run(SampleData.RandomSamples);
runner.Run(SampleData.NegativePositiveRandomSamples);
runner.Run(SampleData.NegativeRandomSamplese);
runner.Run(SampleData.ReversedSamples);
runner.Run(SampleData.MountainSamples);
runner.Run(SampleData.NearlySortedSamples);
runner.Run(SampleData.SortedSamples);
runner.Run(SampleData.SameValuesSamples);
runner.Run(SampleData.DictionarySamples);

Console.WriteLine(runner.MarkDownOutputList.First().Header);
foreach (var item in runner.MarkDownOutputList.Select(x => x.Item))
{
    Console.WriteLine(item);
}

public class Runner
{
    private static int[] validateArray;
    private static KeyValuePair<int, string>[] validateDic;
    public List<IOutput> MarkDownOutputList = [];

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

                // Stooge Sort
                RunSort(new StoogeSort<int>(), item);

                // Too slow....
                if (item.Samples.Length < 100)
                {
                    // Bogo Sort
                    RunSort(new BogoSort<int>(), item);
                }

                if (item.Samples.Length < 1000)
                {
                    // Slow Sort
                    RunSort(new SlowSort<int>(), item);
                }

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

                // Smooth Sort
                RunSort(new SmoothSort<int>(), item);

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

                // DropMerge Sort
                RunSort(new DropMergeSort<int>(), item);

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

                // IntroSort Median9 (Insert + Merge)
                //RunSort(new TimSort<int>(), item);

                // -- Other -- //

                // Pancake Sort
                RunSort(new PancakeSort<int>(), item);
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
        sort.Statistics.IsSorted = after.SequenceEqual(validateArray);

        // result
        MarkDownOutputList.Add(new MarkdownOutput(sort.Statistics, input.InputType));

        // Console Output
        var sortResult = sort.Statistics.IsSorted ? "Correct" : $@"
Before : {keep.ToJoinedString(" ")}
After  : {after.ToJoinedString(" ")}";
        var console = new ConsoleOutput(sort.Statistics, sortResult, input.InputType);
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
        sort.Statistics.IsSorted = after.SequenceEqual(validateArray);

        // result
        MarkDownOutputList.Add(new MarkdownOutput(sort.Statistics, input.InputType));

        // Console Output
        var sortResult = sort.Statistics.IsSorted ? "Correct" : $@"
Before : {keep.ToJoinedString(" ")}
After  : {after.ToJoinedString(" ")}";
        var console = new ConsoleOutput(sort.Statistics, sortResult, input.InputType);
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

public static class SampleData
{
    public static IInputSample<int>[] RandomSamples { get; } = [random, random2, random3];
    public static IInputSample<int>[] NegativePositiveRandomSamples { get; } = [negativePositive, negativePositive2, negativePositive3];
    public static IInputSample<int>[] NegativeRandomSamplese { get; } = [negative, negative2, negative3];
    public static IInputSample<int>[] ReversedSamples { get; } = [reversed, reversed2, reversed3];
    public static IInputSample<int>[] MountainSamples { get; } = [mountain, mountain2, mountain3];
    public static IInputSample<int>[] NearlySortedSamples { get; } = [nearlySorted, nearlySorted2, nearlySorted3];
    public static IInputSample<int>[] SortedSamples { get; } = [sorted, sorted2, sorted3];
    public static IInputSample<int>[] SameValuesSamples { get; } = [sameValues, sameValues2, sameValues3];
    public static IInputSample<int>[] DictionarySamples { get; } = [dictionary, dictionary2, dictionary3];

    private static readonly IInputSample<int> random = new InputSample<int>() { InputType = InputType.Random, Samples = Enumerable.Range(0, 100).Sample(100).ToArray() };
    private static readonly IInputSample<int> random2 = new InputSample<int>() { InputType = InputType.Random, Samples = Enumerable.Range(0, 1000).Sample(1000).ToArray() };
    private static readonly IInputSample<int> random3 = new InputSample<int>() { InputType = InputType.Random, Samples = Enumerable.Range(0, 10000).Sample(10000).ToArray() };

    private static readonly IInputSample<int> negativePositive = new InputSample<int>() { InputType = InputType.MixRandom, Samples = Enumerable.Range(-50, 100).Sample(100).ToArray() };
    private static readonly IInputSample<int> negativePositive2 = new InputSample<int>() { InputType = InputType.MixRandom, Samples = Enumerable.Range(-500, 1000).Sample(1000).ToArray() };
    private static readonly IInputSample<int> negativePositive3 = new InputSample<int>() { InputType = InputType.MixRandom, Samples = Enumerable.Range(-5000, 10000).Sample(10000).ToArray() };

    private static readonly IInputSample<int> negative = new InputSample<int>() { InputType = InputType.NegativeRandom, Samples = Enumerable.Range(-100, 100).Sample(100).ToArray() };
    private static readonly IInputSample<int> negative2 = new InputSample<int>() { InputType = InputType.NegativeRandom, Samples = Enumerable.Range(-1000, 1000).Sample(1000).ToArray() };
    private static readonly IInputSample<int> negative3 = new InputSample<int>() { InputType = InputType.NegativeRandom, Samples = Enumerable.Range(-10000, 10000).Sample(10000).ToArray() };

    private static readonly IInputSample<int> reversed = new InputSample<int>() { InputType = InputType.Reversed, Samples = Enumerable.Range(0, 100).Reverse().ToArray() };
    private static readonly IInputSample<int> reversed2 = new InputSample<int>() { InputType = InputType.Reversed, Samples = Enumerable.Range(0, 1000).Reverse().ToArray() };
    private static readonly IInputSample<int> reversed3 = new InputSample<int>() { InputType = InputType.Reversed, Samples = Enumerable.Range(0, 10000).Reverse().ToArray() };

    private static readonly IInputSample<int> mountain = new InputSample<int>() { InputType = InputType.Mountain, Samples = Enumerable.Range(0, 50).Concat(Enumerable.Range(0, 50).Reverse()).ToArray() };
    private static readonly IInputSample<int> mountain2 = new InputSample<int>() { InputType = InputType.Mountain, Samples = Enumerable.Range(0, 500).Concat(Enumerable.Range(0, 500).Reverse()).ToArray() };
    private static readonly IInputSample<int> mountain3 = new InputSample<int>() { InputType = InputType.Mountain, Samples = Enumerable.Range(0, 5000).Concat(Enumerable.Range(0, 5000).Reverse()).ToArray() };

    private static readonly IInputSample<int> nearlySorted = new InputSample<int>() { InputType = InputType.NearlySorted, Samples = Enumerable.Range(0, 90).Concat(Enumerable.Range(0, 100).Sample(10)).ToArray() };
    private static readonly IInputSample<int> nearlySorted2 = new InputSample<int>() { InputType = InputType.NearlySorted, Samples = Enumerable.Range(0, 990).Concat(Enumerable.Range(0, 1000).Sample(10)).ToArray() };
    private static readonly IInputSample<int> nearlySorted3 = new InputSample<int>() { InputType = InputType.NearlySorted, Samples = Enumerable.Range(0, 9990).Concat(Enumerable.Range(0, 10000).Sample(10)).ToArray() };

    private static readonly IInputSample<int> sorted = new InputSample<int>() { InputType = InputType.Sorted, Samples = Enumerable.Range(0, 100).ToArray() };
    private static readonly IInputSample<int> sorted2 = new InputSample<int>() { InputType = InputType.Sorted, Samples = Enumerable.Range(0, 1000).ToArray() };
    private static readonly IInputSample<int> sorted3 = new InputSample<int>() { InputType = InputType.Sorted, Samples = Enumerable.Range(0, 10000).ToArray() };

    private static readonly int[] randomeSample = Enumerable.Range(0, 100).Sample(10).ToArray();
    private static readonly int[] randomeSample2 = Enumerable.Range(0, 1000).Sample(10).ToArray();
    private static readonly int[] randomeSample3 = Enumerable.Range(0, 10000).Sample(10).ToArray();
    private static readonly IInputSample<int> sameValues = new InputSample<int>() { InputType = InputType.SameValues, Samples = randomeSample.SelectMany(x => Enumerable.Repeat(x, 10)).Sample(100).ToArray() };
    private static readonly IInputSample<int> sameValues2 = new InputSample<int>() { InputType = InputType.SameValues, Samples = randomeSample2.SelectMany(x => Enumerable.Repeat(x, 100)).Sample(1000).ToArray() };
    private static readonly IInputSample<int> sameValues3 = new InputSample<int>() { InputType = InputType.SameValues, Samples = randomeSample3.SelectMany(x => Enumerable.Repeat(x, 1000)).Sample(10000).ToArray() };

    private static int i = 0;
    private static readonly IInputSample<int> dictionary = new InputSample<int>() { InputType = InputType.DictionaryRamdom, DictionarySamples = random.Samples.Select(x => new KeyValuePair<int, string>(x, $"{x / 25}{((char)(65 + (x % 26)))}{i++}")).ToArray() };
    private static readonly IInputSample<int> dictionary2 = new InputSample<int>() { InputType = InputType.DictionaryRamdom, DictionarySamples = random2.Samples.Select(x => new KeyValuePair<int, string>(x, $"{x / 25}{((char)(65 + (x % 26)))}{i++}")).ToArray() };
    private static readonly IInputSample<int> dictionary3 = new InputSample<int>() { InputType = InputType.DictionaryRamdom, DictionarySamples = random3.Samples.Select(x => new KeyValuePair<int, string>(x, $"{x / 25}{((char)(65 + (x % 26)))}{i++}")).ToArray() };
}