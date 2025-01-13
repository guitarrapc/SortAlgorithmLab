namespace SandboxBenchmark;

[ShortRunJob]
[MemoryDiagnoser]
[MinColumn, MaxColumn]
public class ExchangeBogoBenchmark
{
    //[Params(10, 13)]
    [Params(10)]
    public int Number { get; set; }

    private int[] _bogoArray = default!;
    private BogoSort<int> _bogoSort = default!;

    [IterationSetup]
    public void Setup()
    {
        _bogoArray = BenchmarkData.GenIntArray(Number);
        _bogoSort = new();
    }

    [Benchmark]
    public void BogoSort()
    {
        _bogoSort.Sort(_bogoArray);
    }
}
