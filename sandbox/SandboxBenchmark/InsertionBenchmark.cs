namespace SandboxBenchmark;

[ShortRunJob]
[MemoryDiagnoser]
[MinColumn, MaxColumn]
public class InsertionBenchmark
{
    [Params(100, 1000, 10000)]
    public int Number { get; set; }

    private int[] _binaryinsertArray = default!;
    private BinaryInsertSort<int> _binaryinsertSort = default!;

    [IterationSetup]
    public void Setup()
    {
        _binaryinsertArray = BenchmarkData.GenIntArray(Number);
        _binaryinsertSort = new();
    }

    [Benchmark]
    public void BinaryInsertSort()
    {
        _binaryinsertSort.Sort(_binaryinsertArray);
    }
}
