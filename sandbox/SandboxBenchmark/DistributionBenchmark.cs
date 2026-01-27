namespace SandboxBenchmark;

[MemoryDiagnoser]
public class DistributionBenchmark
{
    [Params(100, 256)]
    public int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.Sorted, DataPattern.Reversed, DataPattern.NearlySorted)]
    public DataPattern Pattern { get; set; }

    private int[] _bucketArray = default!;
    private int[] _countingArray = default!;
    private int[] _radixLSD256Sort = default!;
    private int[] _radixLSD10Sort = default!;

    [IterationSetup]
    public void Setup()
    {
        _bucketArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _countingArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _radixLSD256Sort = BenchmarkData.GenerateIntArray(Size, Pattern);
        _radixLSD10Sort = BenchmarkData.GenerateIntArray(Size, Pattern);
    }

    [Benchmark]
    public void BucketSort()
    {
        SortAlgorithm.Algorithms.BucketSort.Sort(_bucketArray.AsSpan(), x => x);
    }

    [Benchmark]
    public void CountingSort()
    {
        SortAlgorithm.Algorithms.CountingSort.Sort(_countingArray.AsSpan(), x => x);
    }

    [Benchmark]
    public void RadixLSD256Sort()
    {
        SortAlgorithm.Algorithms.RadixLSD256Sort.Sort(_radixLSD256Sort.AsSpan());
    }

    [Benchmark]
    public void RadixLSD10Sort()
    {
        SortAlgorithm.Algorithms.RadixLSD10Sort.Sort(_radixLSD10Sort.AsSpan());
    }
}
