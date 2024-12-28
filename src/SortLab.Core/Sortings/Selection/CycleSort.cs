namespace SortLab.Core.Sortings;

/*
Ref span ...

| Method    | Number | Mean         | Error       | StdDev      | Min          | Max          | Allocated |
|---------- |------- |-------------:|------------:|------------:|-------------:|-------------:|----------:|
| CycleSort | 100    |     108.3 us |    134.5 us |     7.37 us |     102.8 us |     116.7 us |     736 B |
| CycleSort | 1000   |   9,077.7 us |  3,789.3 us |   207.70 us |   8,899.3 us |   9,305.7 us |     736 B |
| CycleSort | 10000  | 106,599.4 us | 18,492.0 us | 1,013.61 us | 105,844.3 us | 107,751.4 us |     448 B |
*/

/// <summary>
/// 現在の要素より後ろに、より小さな要素がないか確認しその個数から現在の要素を差し込むインデックスを算出して入れ替え。これを繰り返す。交換回数は常に少ないが、比較回数が莫大になる。
/// </summary>
/// <remarks>
/// stable : no
/// inplace : yes
/// Compare : O(n^2)
/// Swap : O(n^2)
/// </remarks>
/// <typeparam name="T"></typeparam>
public class CycleSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortType SortType => SortType.Selection;
    protected override string Name => nameof(CycleSort<T>);

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        var span = array.AsSpan();
        SortCore(span);

        return array;
    }

    public void Sort(Span<T> span)
    {
        Statistics.Reset(span.Length, SortType, Name);
        SortCore(span);
    }

    private void SortCore(Span<T> span)
    {
        for (var start = 0; start <= span.Length - 2; start++)
        {
            // Compare value
            var tmp = Index(ref span, start);

            // Find position to swap, start base point to find lower element on right side.
            var pos = FindPosition(span, tmp, start);

            // If no swap needed, continue to the next cycle
            if (pos == start) continue;

            // Ignore duplicate
            pos = SkipDuplicates(span, tmp, pos);

            // Perform the initial swap
            Swap(ref Index(ref span, pos), ref tmp);

            // Complete the cycle
            while (pos != start)
            {
                pos = FindPosition(span, tmp, start);
                pos = SkipDuplicates(span, tmp, pos);
                Swap(ref Index(ref span, pos), ref tmp);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int FindPosition(Span<T> span, T value, int start)
    {
        var pos = start;
        for (var i = start + 1; i < span.Length; i++)
        {
            if (Compare(Index(ref span, i), value) < 0)
            {
                pos++;
            }
        }
        return pos;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int SkipDuplicates(Span<T> span, T value, int pos)
    {
        while (Compare(value, Index(ref span, pos)) == 0)
        {
            pos++;
        }
        return pos;
    }
}
