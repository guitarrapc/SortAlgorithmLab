using SortLab.Core;
using SortLab.Core.Logics;

namespace SandboxBenchmark;

[ShortRunJob]
[MemoryDiagnoser]
[MinColumn, MaxColumn]
public class HeapSortBenchmark
{
    [Params(100, 1000, 10000)]
    public int Number { get; set; }

    private int[] _array = default!;
    private HeapSort<int> _heapSort = default!;

    [IterationSetup]
    public void Setup()
    {
        _array = Enumerable.Range(0, Number).Sample(Number).ToArray();
        _heapSort = new();
    }

    [Benchmark]
    public void HeapSort()
    {
        _heapSort.Sort(_array);
    }
}
