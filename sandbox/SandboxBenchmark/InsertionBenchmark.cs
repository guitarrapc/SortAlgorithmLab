namespace SandboxBenchmark;

[ShortRunJob]
[MemoryDiagnoser]
[MinColumn, MaxColumn]
public class InsertionBenchmark
{
    [Params(100, 1000, 10000)]
    public int Number { get; set; }

    private int[] _balancedtreeArray = default!;
    private BalancedTreeSort<int> _balancedtreeSort = default!;

    private int[] _binaryinsertArray = default!;
    private BinaryInsertSort<int> _binaryinsertSort = default!;

    private int[] _binarytreeArray = default!;
    private BinaryTreeSort<int> _binarytreeSort = default!;

    private int[] _insertionArray = default!;
    private InsertionSort<int> _insertionSort = default!;

    [IterationSetup]
    public void Setup()
    {
        _balancedtreeArray = BenchmarkData.GenIntArray(Number);
        _balancedtreeSort = new();

        _binaryinsertArray = BenchmarkData.GenIntArray(Number);
        _binaryinsertSort = new();

        _binarytreeArray = BenchmarkData.GenIntArray(Number);
        _binarytreeSort = new();

        _insertionArray = BenchmarkData.GenIntArray(Number);
        _insertionSort = new();
    }

    [Benchmark]
    public void BalancedTreeSort()
    {
        _balancedtreeSort.Sort(_balancedtreeArray);
    }

    [Benchmark]
    public void BinaryInsertSort()
    {
        _binaryinsertSort.Sort(_binaryinsertArray);
    }

    [Benchmark]
    public void BinaryTreeSort()
    {
        _binarytreeSort.Sort(_binarytreeArray);
    }

    [Benchmark]
    public void InsertionSort()
    {
        _insertionSort.Sort(_insertionArray);
    }
}
