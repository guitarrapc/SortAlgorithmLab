namespace SandboxBenchmark;

[ShortRunJob]
[MemoryDiagnoser]
[MinColumn, MaxColumn]
public class InsertionBenchmark
{
    [Params(100, 1000, 10000)]
    public int Number { get; set; }

    private int[] _balancedbinarytreeArray = default!;
    private int[] _binaryinsertArray = default!;
    private int[] _binarytreeArray = default!;
    private int[] _insertionArray = default!;
    private int[] _shellArrayCiura2001 = default!;
    private int[] _shellArrayKnuth1973 = default!;
    private int[] _shellArrayLee2021 = default!;
    private int[] _shellArraySedgewick1986 = default!;
    private int[] _shellArrayTokuda1992 = default!;

    [IterationSetup]
    public void Setup()
    {
        _balancedbinarytreeArray = BenchmarkData.GenIntArray(Number);
        _binaryinsertArray = BenchmarkData.GenIntArray(Number);
        _binarytreeArray = BenchmarkData.GenIntArray(Number);
        _insertionArray = BenchmarkData.GenIntArray(Number);
        _shellArrayCiura2001 = BenchmarkData.GenIntArray(Number);
        _shellArrayKnuth1973 = BenchmarkData.GenIntArray(Number);
        _shellArrayLee2021 = BenchmarkData.GenIntArray(Number);
        _shellArraySedgewick1986 = BenchmarkData.GenIntArray(Number);
        _shellArrayTokuda1992 = BenchmarkData.GenIntArray(Number);
    }

    [Benchmark]
    public void BalancedBinaryTreeSort()
    {
        SortAlgorithm.Algorithms.BalancedBinaryTreeSort.Sort(_balancedbinarytreeArray.AsSpan());
    }

    [Benchmark]
    public void BinaryInsertSort()
    {
        SortAlgorithm.Algorithms.BinaryInsertSort.Sort(_binaryinsertArray.AsSpan());
    }

    [Benchmark]
    public void BinaryTreeSort()
    {
        SortAlgorithm.Algorithms.BinaryTreeSort.Sort(_binarytreeArray.AsSpan());
    }

    [Benchmark]
    public void InsertionSort()
    {
        SortAlgorithm.Algorithms.InsertionSort.Sort(_insertionArray.AsSpan());
    }

    [Benchmark]
    public void ShellSortCiura2001()
    {
        SortAlgorithm.Algorithms.ShellSortCiura2001.Sort(_shellArrayCiura2001.AsSpan());
    }

    [Benchmark]
    public void ShellSortKnuth1973()
    {
        SortAlgorithm.Algorithms.ShellSortKnuth1973.Sort(_shellArrayKnuth1973.AsSpan());
    }

    [Benchmark]
    public void ShellSortLee2021()
    {
        SortAlgorithm.Algorithms.ShellSortLee2021.Sort(_shellArrayLee2021.AsSpan());
    }

    [Benchmark]
    public void ShellSortSedgewick1986()
    {
        SortAlgorithm.Algorithms.ShellSortSedgewick1986.Sort(_shellArraySedgewick1986.AsSpan());
    }

    [Benchmark]
    public void ShellSortTokuda1992()
    {
        SortAlgorithm.Algorithms.ShellSortTokuda1992.Sort(_shellArrayTokuda1992.AsSpan());
    }
}
