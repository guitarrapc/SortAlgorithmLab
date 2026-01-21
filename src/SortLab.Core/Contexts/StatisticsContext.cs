namespace SortLab.Core.Contexts;

/// <summary>
/// Tracks the number of comparisons, swaps, and index accesses performed during sorting operations for a collection of elements.
/// </summary>
/// <remarks>
/// Use this context to gather statistics when implementing or analyzing sorting algorithms. The
/// collected counts can be used to evaluate algorithm efficiency or compare different sorting strategies. This class is
/// thread-safe for incrementing statistics.
/// </remarks>
public sealed class StatisticsContext : ISortContext
{
    public ulong CompareCount => _compareCount;
    private ulong _compareCount;

    public ulong SwapCount => _swapCount;
    private ulong _swapCount;

    public ulong IndexAccessCount => _indexAccessCount;
    private ulong _indexAccessCount;

    public void OnCompare(int i, int j, int result) => Interlocked.Increment(ref _compareCount);
    public void OnSwap(int i, int j) => Interlocked.Increment(ref _swapCount);
    public void OnIndexAccess(int index) => Interlocked.Increment(ref _indexAccessCount);

    /// <summary>
    /// Resets all operation counters to zero.
    /// </summary>
    /// <remarks>
    /// Call this method to clear the current counts for comparisons, swaps, and index accesses,
    /// typically before starting a new measurement or operation.
    /// </remarks>
    public void Reset()
    {
        _compareCount = 0;
        _swapCount = 0;
        _indexAccessCount = 0;
    }
}
