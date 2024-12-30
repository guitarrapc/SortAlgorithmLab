namespace SandboxBenchmark;

[ShortRunJob]
[MemoryDiagnoser]
[MinColumn, MaxColumn]
public class InsertionBenchmark
{
    [Params(100, 1000, 10000)]
    public int Number { get; set; }

    private int[] _balancedbinarytreeArray = default!;
    private BalancedBinaryTreeSort<int> _balancedbinarytreeSort = default!;

    private int[] _binaryinsertArray = default!;
    private BinaryInsertSort<int> _binaryinsertSort = default!;

    private int[] _binarytreeArray = default!;
    private BinaryTreeSort<int> _binarytreeSort = default!;

    private int[] _insertionArray = default!;
    private InsertionSort<int> _insertionSort = default!;

    private int[] _shellArray = default!;
    private ShellSort<int> _shellSort = default!;

    [IterationSetup]
    public void Setup()
    {
        _balancedbinarytreeArray = BenchmarkData.GenIntArray(Number);
        _balancedbinarytreeSort = new();

        _binaryinsertArray = BenchmarkData.GenIntArray(Number);
        _binaryinsertSort = new();

        _binarytreeArray = BenchmarkData.GenIntArray(Number);
        _binarytreeSort = new();

        _insertionArray = BenchmarkData.GenIntArray(Number);
        _insertionSort = new();

        _shellArray = BenchmarkData.GenIntArray(Number);
        _shellSort = new();
    }

    [Benchmark]
    public void BalancedBinaryTreeSort()
    {
        _balancedbinarytreeSort.Sort(_balancedbinarytreeArray);
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

    [Benchmark]
    public void ShellSort()
    {
        _shellSort.Sort(_shellArray);
    }
}
