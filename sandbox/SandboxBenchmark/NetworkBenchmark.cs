namespace SandboxBenchmark;

[MemoryDiagnoser]
public class NetworkBenchmark
{
    [Params(100, 1000, 10000)]
    public int Size { get; set; }

    [Params(DataPattern.Random, DataPattern.Sorted, DataPattern.Reversed, DataPattern.NearlySorted)]
    public DataPattern Pattern { get; set; }

    private int[] _bionicArray = default!;
    private int[] _bionicParallelArray = default!;

    [IterationSetup]
    public void Setup()
    {
        _bionicArray = BenchmarkData.GenerateIntArray(Size, Pattern);
        _bionicParallelArray = BenchmarkData.GenerateIntArray(Size, Pattern);
    }

    [Benchmark]
    public void BitonicSort()
    {
        SortAlgorithm.Algorithms.BitonicSort.Sort(_bionicArray.AsSpan());
    }

    [Benchmark]
    public void BitonicSortParallelSort()
    {
        SortAlgorithm.Algorithms.BitonicSortParallel.Sort(_bionicArray);
    }
}
