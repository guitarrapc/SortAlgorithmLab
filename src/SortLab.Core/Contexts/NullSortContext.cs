namespace SortLab.Core.Contexts;

/// <summary>
/// No-op implementation of ISortContext.
/// </summary>
public sealed class NullSortContext : ISortContext
{
    public static readonly NullSortContext Default = new();

    private NullSortContext()
    {
    }

    public void OnCompare(int i, int j, int result)
    {
    }
    public void OnSwap(int i, int j)
    {
    }
    public void OnIndexAccess(int index)
    {
    }
}
