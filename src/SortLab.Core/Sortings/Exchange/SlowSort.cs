namespace SortLab.Core.Sortings;

/// <summary>
/// 配列左半分で最大の数をさがし、配列右半分で最大の数を探して両者を比較し左半分が大きければ交換。残りを配列の要素数-1で繰り返しソート済みまで行く。
/// Multiply and Surrender戦略を使った分割統治アルゴリズムで、意図的に非効率な設計のため実用性はない。
/// </summary>
/// <remarks>
/// stable  : no
/// inplace : yes
/// Compare : Ω(n^(log n / (2+ε)))  (Extremely high number of comparisons due to recursive overhead)
/// Swap    : O(n^(log n / 2))     (Depends on the data, but can be very high)
/// Order   : O(n^(log n / (2+ε))) on average (Worst case: much worse than O(n^2), one of the slowest sorting algorithms)
/// </remarks>
/// <typeparam name="T"></typeparam>
public class SlowSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Exchange;
    protected override string Name => nameof(SlowSort<T>);

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

        var m = (start + end) / 2;
        SortCore(span, start, m);
        SortCore(span, m + 1, end);

        if (Compare(Index(span, end), Index(span, m)) < 0)
        {
            Swap(ref Index(span, end), ref Index(span, m));
        }
        SortCore(span, start, end - 1);
    }
}
