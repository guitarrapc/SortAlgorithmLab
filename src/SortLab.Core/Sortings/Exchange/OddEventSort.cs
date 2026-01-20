namespace SortLab.Core.Sortings;

/// <summary>
/// <see cref="BubbleSort{T}"/>の改良版。BubbleSortの一方通行スキャンを、奇数偶数と偶数奇数のそれぞれの組ごとに行う。組みは互いに独立しているので並列動作が可能なので、ハードウェア次第でn-1ステップの処理が可能。安定な内部ソート
/// 単純だが低速
/// </summary>
/// <remarks>
/// stable : yes
/// inplace : yes
/// Compare : n(n-1) / 2
/// Swap : Average n(n-1)/4
/// Order : O(n^2)
/// </remarks>
/// <typeparam name="T"></typeparam>
public class OddEvenSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Exchange;
    protected override string Name => nameof(OddEvenSort<T>);

    public override void Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        SortCore(array.AsSpan());
    }

    public override void Sort(Span<T> span)
    {
        Statistics.Reset(span.Length, SortType, Name);
        SortCore(span);
    }

    private void SortCore(Span<T> span)
    {
        var sorted = false;
        while (!sorted)
        {
            sorted = true;

            // odd-even ({1,2},{3,4}) sort
            for (var i = 0; i < span.Length - 1; i += 2)
            {
                if (Compare(Index(span, i), Index(span, i + 1)) > 0)
                {
                    Swap(ref Index(span, i), ref Index(span, i + 1));
                    sorted = false;
                }
            }

            // even-odd ({2,3},{4,5}) sort
            for (var i = 1; i < span.Length - 1; i += 2)
            {
                if (Compare(Index(span, i), Index(span, i + 1)) > 0)
                {
                    Swap(ref Index(span, i), ref Index(span, i + 1));
                    sorted = false;
                }
            }
        }
    }
}
