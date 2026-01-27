namespace SandboxBenchmark;

[MemoryDiagnoser]
public class AdaptiveBenchmark
{
    [Params(100, 1000, 10000)]
    public int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.Sorted, DataPattern.Reversed, DataPattern.NearlySorted)]
    public DataPattern Pattern { get; set; }

    private int[] _dropMergeArray = default!;

    [IterationSetup]
    public void Setup()
    {
        _dropMergeArray = BenchmarkData.GenerateIntArray(Size, Pattern);
    }

    [Benchmark]
    public void DropMergeSort()
    {
        SortAlgorithm.Algorithms.DropMergeSort.Sort(_dropMergeArray.AsSpan());
    }
}
