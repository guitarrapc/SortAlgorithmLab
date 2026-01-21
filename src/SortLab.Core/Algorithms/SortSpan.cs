using SortLab.Core.Contexts;

namespace SortLab.Core.Algorithms;

internal ref struct SortSpan<T>(Span<T> span, ISortContext ccontext) where T: IComparable<T>
{
    private Span<T> _span = span;
    private readonly ISortContext _context = ccontext;

    public int Length => _span.Length;

    /// <summary>
    /// Retrieves the element at the specified zero-based index. (Equivalent to span[i].)
    /// </summary>
    /// <param name="i">The zero-based index of the element to retrieve. Must be within the bounds of the collection.</param>
    /// <returns>The element of type T at the specified index.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Read(int i)
    {
        _context.OnIndexRead(i);
        return _span[i];
    }

    /// <summary>
    /// Sets the element at the specified index to the given value. (Equivalent to span[i] = value.)
    /// </summary>
    /// <param name="i">The zero-based index of the element to set.</param>
    /// <param name="value">The value to assign to the element at the specified index.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(int i, T value)
    {
        _context.OnIndexWrite(i);
        _span[i] = value;
    }

    /// <summary>
    /// Compares the elements at the specified indices and returns an integer that indicates their relative order. (Equivalent to span[i].CompareTo(span[j]).)
    /// </summary>
    /// <param name="i">The index of the first element to compare.</param>
    /// <param name="j">The index of the second element to compare.</param>
    /// <returns>A signed integer that indicates the relative order of the elements: less than zero if the element at index i is
    /// less than the element at index j; zero if they are equal; greater than zero if the element at index i is greater
    /// than the element at index j.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Compare(int i, int j)
    {
        var a = Read(i);
        var b = Read(j);
        var result = a.CompareTo(b);
        _context.OnCompare(i, j, result);
        return result;
    }

    /// <summary>
    /// Compares the element at the specified index with a given value. (Equivalent to span[i].CompareTo(value).)
    /// </summary>
    /// <param name="i">The index of the element to compare.</param>
    /// <param name="value">The value to compare against.</param>
    /// <returns>A signed integer that indicates the relative order: less than zero if the element at index i is
    /// less than value; zero if they are equal; greater than zero if the element at index i is greater than value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Compare(int i, T value)
    {
        var a = Read(i);
        var result = a.CompareTo(value);
        _context.OnCompare(i, -1, result);
        return result;
    }

    /// <summary>
    /// Compares a given value with the element at the specified index. (Equivalent to value.CompareTo(span[i]).)
    /// </summary>
    /// <param name="value">The value to compare.</param>
    /// <param name="i">The index of the element to compare against.</param>
    /// <returns>A signed integer that indicates the relative order: less than zero if value is
    /// less than the element at index i; zero if they are equal; greater than zero if value is greater than the element at index i.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Compare(T value, int i)
    {
        var b = Read(i);
        var result = value.CompareTo(b);
        _context.OnCompare(-1, i, result);
        return result;
    }

    /// <summary>
    /// Exchanges the values at the specified indices within the collection. (Equivalent to swapping span[i] and span[j].)
    /// </summary>
    /// <remarks>This method notifies the underlying context of the swap operation before updating the values.
    /// Both indices must refer to valid elements within the collection.</remarks>
    /// <param name="i">The index of the first element to swap.</param>
    /// <param name="j">The index of the second element to swap.</param>
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
