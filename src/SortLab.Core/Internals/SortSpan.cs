using SortLab.Core.Contexts;

namespace SortLab.Core.Internals;

internal ref struct SortSpan<T>(Span<T> span, ISortContext ccontext) where T: IComparable<T>
{
    private Span<T> _span = span;
    private readonly ISortContext _context = ccontext;

    public int Length => _span.Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Read(int i)
    {
        _context.OnIndexAccess(i);
        return _span[i];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(int i, T value)
    {
        _context.OnIndexAccess(i);
        _span[i] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Compare(int i, int j)
    {
        var a = Read(i);
        var b = Read(j);
        var result = a.CompareTo(b);
        _context.OnCompare(i, j, result);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Swap(int i, int j)
    {
        var a = Read(i);
        var b = Read(j);

        _context.OnSwap(i, j);

        Write(i, b);
        Write(j, a);
    }
}
