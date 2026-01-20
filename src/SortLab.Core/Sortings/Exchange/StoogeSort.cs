namespace SortLab.Core.Sortings;

/// <summary>
/// 配列の先頭と末尾を比較し先頭が大きければ交換。処理配列が3要素以上なら先頭2/3、末尾2/3、先頭2/3の順にソート。激遅、不安定で外部メモリと配列の同長つかうだめだめソート。
/// </summary>
/// <remarks>
/// stable : no
/// inplace : no (n)
/// Compare : n^(log3/log1.5)
/// Swap : n^2
/// Order : O(n^(log3/log1.5)) = O(n^2.7095)
/// </remarks>
/// <typeparam name="T"></typeparam>
public class StoogeSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Exchange;
    protected override string Name => nameof(StoogeSort<T>);

    public override void Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        SortCore(array.AsSpan(), 0, array.Length - 1);
    }

    public override void Sort(Span<T> span)
    {
        Statistics.Reset(span.Length, SortType, Name);
        SortCore(span, 0, span.Length - 1);
    }

    private void SortCore(Span<T> span, int start, int end)
    {
        if (start >= end) return;

        // If first element is larger than last, swap them
        if (Compare(Index(span, start), Index(span, end)) > 0)
        {
            Swap(ref Index(span, start), ref Index(span, end));
        }

        // If there are 3 or more elements
        if (end - start + 1 >= 3)
        {
            var third = (end - start + 1) / 3;

            // Sort first 2/3
            SortCore(span, start, end - third);

            // Sort last 2/3
            SortCore(span, start + third, end);

            // Sort first 2/3 again
            SortCore(span, start, end - third);
        }
    }
}
