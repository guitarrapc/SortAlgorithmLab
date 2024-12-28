using SortLab.Core;
using SortLab.Core.Sortings;
using System;
using System.Collections.Generic;
using System.Linq;

var runner = new Runner()
{
    //Distribution = true,
    //Exchange = true,
    //Hybrid = true,
    //Insertion = true,
    //Merge = true,
    //Other = true,
    //Partition = true,
    Selection = true,
};
runner.Run(SampleData.RandomSamples);
runner.Run(SampleData.NegativePositiveRandomSamples);
runner.Run(SampleData.NegativeRandomSamples);
runner.Run(SampleData.ReversedSamples);
runner.Run(SampleData.MountainSamples);
runner.Run(SampleData.NearlySortedSamples);
runner.Run(SampleData.SortedSamples);
runner.Run(SampleData.SameValuesSamples);
runner.Run(SampleData.DictionarySamples);

Console.WriteLine(runner.MarkDownOutputList[0].Header);
foreach (var item in runner.MarkDownOutputList.Select(x => x.Item))
{
    Console.WriteLine(item);
}

public record Runner
{
    public bool Distribution { get; init; }
    public bool Exchange { get; init; }
    public bool Hybrid { get; init; }
    public bool Insertion { get; init; }
    public bool Merge { get; init; }
    public bool Other { get; init; }
    public bool Partition { get; init; }
    public bool Selection { get; init; }

    private static int[] validateArray;
    private static CustomKeyValuePair<int, string>[] validateDic;
    public List<IOutput> MarkDownOutputList = [];

    public void Run(IInputSample<int>[] items)
    {
        foreach (var item in items)
        {
            if (item.Samples != null)
            {
                Init(item.Samples);

                if (Distribution)
                {
                    // -- Distribution -- //

                    // Bucket Sort
                    var bucketSort = new BucketSortInt<int>();
                    RunSort(bucketSort, array => bucketSort.Sort(array), item);

                    // Radix Sort(LSD)
                    var radix10Sort = new RadixLSD10Sort<int>();
                    RunSort(radix10Sort, array => radix10Sort.Sort(array), item);

                    var radix4Sort = new RadixLSD4Sort<int>();
                    RunSort(radix4Sort, array => radix4Sort.Sort(array), item);

                    // Counting Sort
                    var countingSort = new CountingSort<int>();
                    RunSort(countingSort, array => countingSort.Sort(array), item);
                }

                if (Exchange)
                {
                    // -- Exchange -- //

                    if (item.Samples.Length < 100)
                    {
                        // Bogo Sort (Too slow....)
                        RunSort(new BogoSort<int>(), item);
                    }

                    // Bubble Sort
                    RunSort(new BubbleSort<int>(), item);

                    // Cocktail Shaker Sort
                    RunSort(new CocktailShakerSort<int>(), item);
                    RunSort(new CocktailShakerSort2<int>(), item);

                    // Comb Sort
                    RunSort(new CombSort<int>(), item);

                    // Gnome Sort
                    RunSort(new GnomeSort<int>(), item);
                    RunSort(new GnomeSort1<int>(), item);
                    RunSort(new GnomeSort2<int>(), item);
                    RunSort(new GnomeSort3<int>(), item);

                    // OddEven Sort
                    RunSort(new OddEvenSort<int>(), item);

                    if (item.Samples.Length < 1000)
                    {
                        // Slow Sort (Too slow....)
                        RunSort(new SlowSort<int>(), item);
                    }

                    // Stooge Sort
                    RunSort(new StoogeSort<int>(), item);
                }

                if (Hybrid)
                {
                    // -- Hybrid -- //

                    // IntroSort Median3 (Quick + Heap + Insert)
                    //RunSort(new IntroSortMedian3<int>(), item);

                    // IntroSort Median9 (Quick + Heap + Insert)
                    RunSort(new IntroSortMedian9<int>(), item);

                    // IntroSort Median9 (Insert + Merge)
                    //RunSort(new TimSort<int>(), item);
                }

                if (Insertion)
                {
                    // -- Insertion -- //

                    // Binary Insert Sort
                    RunSort(new BinaryInsertSort<int>(), item);

                    // Binary Tree Sort
                    RunSort(new BinaryTreeSort<int>(), item);

                    // Insert Sort
                    RunSort(new InsertSort<int>(), item);

                    // Shell Sort
                    RunSort(new ShellSort<int>(), item);
                }

                if (Merge)
                {
                    // -- Merge -- //

                    // DropMerge Sort
                    RunSort(new DropMergeSort<int>(), item);

                    // Merge Sort
                    RunSort(new MergeSort<int>(), item);
                    RunSort(new MergeSort2<int>(), item);

                    // Shift Sort
                    RunSort(new ShiftSort<int>(), item);
                }

                if (Other)
                {
                    // -- Other -- //

                    // Pancake Sort
                    RunSort(new PancakeSort<int>(), item);
                }

                if (Partition)
                {
                    // -- Partitionig -- //

                    // QuickSort Dual Pivot
                    RunSort(new QuickSortDualPivot<int>(), item);

                    // Quick Sort Dual Pivot (Quick + BinaryInsert)
                    RunSort(new QuickSortDualPivotWithBinaryInsert<int>(), item);

                    // Quick Sort Dual Pivot(Quick + Insert)
                    RunSort(new QuickSortDualPivotWithInsert<int>(), item);

                    // Quick Sort Median3
                    RunSort(new QuickSortMedian3<int>(), item);

                    // QuickSort Median3 (Quick + BinaryInsert)
                    RunSort(new QuickSortMedian3WithBinaryInsert<int>(), item);

                    // QuickSort Median3 (Quick + Insert)
                    RunSort(new QuickSortMedian3WithInsert<int>(), item);

                    // Quick Sort Median9
                    RunSort(new QuickSortMedian9<int>(), item);

                    // QuickSort Median9 (Quick + BinaryInsert)
                    RunSort(new QuickSortMedian9WithBinaryInsert<int>(), item);

                    // QuickSort Median9 (Quick + Insert)
                    RunSort(new QuickSortMedian9WithInsert<int>(), item);
                }

                if (Selection)
                {
                    // -- Selection -- //

                    // Cycle Sort
                    RunSort(new CycleSort<int>(), item);

                    // Heap Sort
                    RunSort(new HeapSort<int>(), item);

                    // Selection Sort
                    RunSort(new SelectionSort<int>(), item);

                    // Smooth Sort
                    RunSort(new SmoothSort<int>(), item);
                }
            }
            else
            {
                if (item.DictionarySamples != null)
                {
                    Init(item.DictionarySamples);

                    if (Distribution)
                    {
                        // -- Distribution -- //

                        // BucketSort<T>
                        RunBucketTSort(new BucketSort<CustomKeyValuePair<int, string>>(x => x.Key), item.DictionarySamples.Max(x => x.Key), item);
                    }
                }
            }
        }
    }

    private void Init(int[] input)
    {
        validateArray = input.OrderBy(x => x).ToArray();
    }

    private void Init(CustomKeyValuePair<int, string>[] array)
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
        var sortResult = sort.Statistics.IsSorted ? "Correct" : $"""
        Before : {keep.ToJoinedString(" ")}
        After  : {after.ToJoinedString(" ")}
        """;
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
        var sortResult = sort.Statistics.IsSorted ? "Correct" : $"""
        Before : {keep.ToJoinedString(" ")}
        After  : {after.ToJoinedString(" ")}
        """;
        var console = new ConsoleOutput(sort.Statistics, sortResult, input.InputType);
        Console.WriteLine(console.ToString());

        // reset
        ResetArray(ref keep, ref array);
    }

    private void RunBucketTSort(BucketSort<CustomKeyValuePair<int, string>> sort, int max, IInputSample<int> input)
    {
        var array = input.DictionarySamples;

        // prerequites
        var keep = new CustomKeyValuePair<int, string>[array.Length];
        ResetArray(ref array, ref keep);

        // run sort
        var after = sort.Sort(array);

        // validate
        sort.Statistics.IsSorted = after.SequenceEqual(validateDic);

        // result
        MarkDownOutputList.Add(new MarkdownOutput(sort.Statistics, input.InputType));

        // Console Output
        var sortResult = sort.Statistics.IsSorted ? "Correct" : $"""
        Before : {keep.ToJoinedString(" ")}
        After  : {after.ToJoinedString(" ")}
        """;
        var console = new ConsoleOutput(sort.Statistics, sortResult, input.InputType);
        Console.WriteLine(console.ToString());

        // reset
        ResetArray(ref keep, ref array);
    }

    private void ResetArray<T>(ref T[] source, ref T[] target) where T : IComparable
    {
        source.CopyTo(target, 0);
    }

    private void ResetArray<TKey, TValue>(ref CustomKeyValuePair<TKey, TValue>[] source, ref CustomKeyValuePair<TKey, TValue>[] target) where TKey : IComparable
    {
        source.CopyTo(target, 0);
    }
}

public static class SampleData
{
    static SampleData()
    {
        var random = new InputSample<int>() { InputType = InputType.Random, Samples = Enumerable.Range(0, 100).Sample(100).ToArray() };
        var random2 = new InputSample<int>() { InputType = InputType.Random, Samples = Enumerable.Range(0, 1000).Sample(1000).ToArray() };
        var random3 = new InputSample<int>() { InputType = InputType.Random, Samples = Enumerable.Range(0, 10000).Sample(10000).ToArray() };

        var negativePositive = new InputSample<int>() { InputType = InputType.MixRandom, Samples = Enumerable.Range(-50, 100).Sample(100).ToArray() };
        var negativePositive2 = new InputSample<int>() { InputType = InputType.MixRandom, Samples = Enumerable.Range(-500, 1000).Sample(1000).ToArray() };
        var negativePositive3 = new InputSample<int>() { InputType = InputType.MixRandom, Samples = Enumerable.Range(-5000, 10000).Sample(10000).ToArray() };

        var negative = new InputSample<int>() { InputType = InputType.NegativeRandom, Samples = Enumerable.Range(-100, 100).Sample(100).ToArray() };
        var negative2 = new InputSample<int>() { InputType = InputType.NegativeRandom, Samples = Enumerable.Range(-1000, 1000).Sample(1000).ToArray() };
        var negative3 = new InputSample<int>() { InputType = InputType.NegativeRandom, Samples = Enumerable.Range(-10000, 10000).Sample(10000).ToArray() };

        var reversed = new InputSample<int>() { InputType = InputType.Reversed, Samples = Enumerable.Range(0, 100).Reverse().ToArray() };
        var reversed2 = new InputSample<int>() { InputType = InputType.Reversed, Samples = Enumerable.Range(0, 1000).Reverse().ToArray() };
        var reversed3 = new InputSample<int>() { InputType = InputType.Reversed, Samples = Enumerable.Range(0, 10000).Reverse().ToArray() };

        var mountain = new InputSample<int>() { InputType = InputType.Mountain, Samples = Enumerable.Range(0, 50).Concat(Enumerable.Range(0, 50).Reverse()).ToArray() };
        var mountain2 = new InputSample<int>() { InputType = InputType.Mountain, Samples = Enumerable.Range(0, 500).Concat(Enumerable.Range(0, 500).Reverse()).ToArray() };
        var mountain3 = new InputSample<int>() { InputType = InputType.Mountain, Samples = Enumerable.Range(0, 5000).Concat(Enumerable.Range(0, 5000).Reverse()).ToArray() };

        var nearlySorted = new InputSample<int>() { InputType = InputType.NearlySorted, Samples = Enumerable.Range(0, 90).Concat(Enumerable.Range(0, 100).Sample(10)).ToArray() };
        var nearlySorted2 = new InputSample<int>() { InputType = InputType.NearlySorted, Samples = Enumerable.Range(0, 990).Concat(Enumerable.Range(0, 1000).Sample(10)).ToArray() };
        var nearlySorted3 = new InputSample<int>() { InputType = InputType.NearlySorted, Samples = Enumerable.Range(0, 9990).Concat(Enumerable.Range(0, 10000).Sample(10)).ToArray() };

        var sorted = new InputSample<int>() { InputType = InputType.Sorted, Samples = Enumerable.Range(0, 100).ToArray() };
        var sorted2 = new InputSample<int>() { InputType = InputType.Sorted, Samples = Enumerable.Range(0, 1000).ToArray() };
        var sorted3 = new InputSample<int>() { InputType = InputType.Sorted, Samples = Enumerable.Range(0, 10000).ToArray() };

        var randomeSample = Enumerable.Range(0, 100).Sample(10).ToArray();
        var randomeSample2 = Enumerable.Range(0, 1000).Sample(10).ToArray();
        var randomeSample3 = Enumerable.Range(0, 10000).Sample(10).ToArray();
        var sameValues = new InputSample<int>() { InputType = InputType.SameValues, Samples = randomeSample.SelectMany(x => Enumerable.Repeat(x, 10)).Sample(100).ToArray() };
        var sameValues2 = new InputSample<int>() { InputType = InputType.SameValues, Samples = randomeSample2.SelectMany(x => Enumerable.Repeat(x, 100)).Sample(1000).ToArray() };
        var sameValues3 = new InputSample<int>() { InputType = InputType.SameValues, Samples = randomeSample3.SelectMany(x => Enumerable.Repeat(x, 1000)).Sample(10000).ToArray() };

        var i = 0;
        var dictionary = new InputSample<int>() { InputType = InputType.DictionaryRamdom, DictionarySamples = random.Samples.Select(x => new CustomKeyValuePair<int, string>(x, $"{x / 25}{((char)(65 + (x % 26)))}{i++}")).ToArray() };
        var dictionary2 = new InputSample<int>() { InputType = InputType.DictionaryRamdom, DictionarySamples = random2.Samples.Select(x => new CustomKeyValuePair<int, string>(x, $"{x / 25}{((char)(65 + (x % 26)))}{i++}")).ToArray() };
        var dictionary3 = new InputSample<int>() { InputType = InputType.DictionaryRamdom, DictionarySamples = random3.Samples.Select(x => new CustomKeyValuePair<int, string>(x, $"{x / 25}{((char)(65 + (x % 26)))}{i++}")).ToArray() };

        RandomSamples = [random, random2, random3];
        NegativePositiveRandomSamples = [negativePositive, negativePositive2, negativePositive3];
        NegativeRandomSamples = [negative, negative2, negative3];
        ReversedSamples = [reversed, reversed2, reversed3];
        MountainSamples = [mountain, mountain2, mountain3];
        NearlySortedSamples = [nearlySorted, nearlySorted2, nearlySorted3];
        SortedSamples = [sorted, sorted2, sorted3];
        SameValuesSamples = [sameValues, sameValues2, sameValues3];
        DictionarySamples = [dictionary, dictionary2, dictionary3];
    }
    public static IInputSample<int>[] RandomSamples { get; }
    public static IInputSample<int>[] NegativePositiveRandomSamples { get; }
    public static IInputSample<int>[] NegativeRandomSamples { get; }
    public static IInputSample<int>[] ReversedSamples { get; }
    public static IInputSample<int>[] MountainSamples { get; }
    public static IInputSample<int>[] NearlySortedSamples { get; }
    public static IInputSample<int>[] SortedSamples { get; }
    public static IInputSample<int>[] SameValuesSamples { get; }
    public static IInputSample<int>[] DictionarySamples { get; }

}