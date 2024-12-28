using SortLab.Core;
using SortLab.Core.Sortings;

namespace SandboxBenchmark;

[ShortRunJob]
[MemoryDiagnoser]
[MinColumn, MaxColumn]
public class SelectionBenchmark
{
    [Params(100, 1000, 10000)]
    public int Number { get; set; }

    private int[] _cycleArray = default!;
    private CycleSort<int> _cycleSort = default!;
    private int[] _heapArray = default!;
    private HeapSort<int> _heapSort = default!;
    private int[] _selectionArray = default!;
    private SelectionSort<int> _selectionSort = default!;
    private int[] _smoothArray = default!;
    private SmoothSort<int> _smoothSort = default!;

    [IterationSetup]
    public void Setup()
    {
        _cycleArray = Enumerable.Range(0, Number).Sample(Number).ToArray();
        _cycleSort = new();

        _heapArray = _cycleArray.ToArray();
        _heapSort = new();

        _selectionArray = _cycleArray.ToArray();
        _selectionSort = new();

        _smoothArray = _cycleArray.ToArray();
        _smoothSort = new();
    }

    [Benchmark]
    public void CycleSort()
    {
        _cycleSort.Sort(_cycleArray);
    }

    [Benchmark]
    public void HeapSort()
    {
        _heapSort.Sort(_heapArray);
    }

    [Benchmark]
    public void SelectionSort()
    {
        _selectionSort.Sort(_selectionArray);
    }

    [Benchmark]
    public void SmoothSort()
    {
        _smoothSort.Sort(_smoothArray);
    }
}
