namespace SortLab.Core.Contexts;

/// <summary>
/// Provides a context for visualizing sorting operations by exposing callbacks for compare, swap, and index access
/// events.
/// </summary>
/// <remarks>
/// Use this class to observe or record the behavior of sorting algorithms by supplying callback actions
/// for key operations. This is useful for building visualizations or collecting statistics during sorting. The class is
/// sealed and intended for use as a utility within sorting visualizations or analysis tools.
/// </remarks>
public sealed class VisualizationContext : ISortContext
{
    private readonly Action<int, int, int>? _onCompare;
    private readonly Action<int, int>? _onSwap;
    private readonly Action<int>? _onIndexAccess;

    public VisualizationContext(
        Action<int, int, int>? onCompare = null,
        Action<int, int>? onSwap = null,
        Action<int>? onIndexAccess = null)
    {
        _onCompare = onCompare;
        _onSwap = onSwap;
        _onIndexAccess = onIndexAccess;
    }

    public void OnCompare(int i, int j, int result) => _onCompare?.Invoke(i, j, result);
    public void OnSwap(int i, int j) => _onSwap?.Invoke(i, j);
    public void OnIndexAccess(int index) => _onIndexAccess?.Invoke(index);
}
