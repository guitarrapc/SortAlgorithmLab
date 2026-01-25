namespace SandboxBenchmark;

[ShortRunJob]
[MemoryDiagnoser]
[MinColumn, MaxColumn]
public class SelectionBenchmark
{
    [Params(100, 1000, 10000)]
    public int Number { get; set; }

    private int[] _cycleArray = default!;
    private int[] _pancakeArray = default!;
    private int[] _selectionArray = default!;

    [IterationSetup]
    public void Setup()
    {
        _cycleArray = BenchmarkData.GenIntArray(Number);
        _pancakeArray = BenchmarkData.GenIntArray(Number);
        _selectionArray = BenchmarkData.GenIntArray(Number);
    }

    [Benchmark]
    public void CycleSort()
    {
        SortLab.Core.Algorithms.CycleSort.Sort(_cycleArray);
    }

    [Benchmark]
    public void PancakeSort()
    {
        SortLab.Core.Algorithms.PancakeSort.Sort(_pancakeArray);
    }

    [Benchmark]
    public void SelectionSort()
    {
        SortLab.Core.Algorithms.SelectionSort.Sort(_selectionArray);
    }
}
