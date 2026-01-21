namespace SortLab.Core.Contexts;

/// <summary>
/// No-op implementation of ISortContext.
/// </summary>
public sealed class NullContext : ISortContext
{
    public static readonly NullContext Default = new();

    private NullContext()
    {
    }

    public void OnCompare(int i, int j, int result)
    {
    }

    public void OnSwap(int i, int j)
    {
    }

    public void OnIndexRead(int index)
    {
    }

    public void OnIndexWrite(int index)
    {
    }
}
