namespace SortLab.Core.Sortings;

/// <summary>
/// リストの先頭に戻る前に、前回の位置を覚えておくことで<see cref="GnomeSortWithSwap{T}"/>を最適化している。これにより、<see cref="InsertionSort{T}"/>と同程度の計算量になる。
/// </summary>
/// <remarks>
/// stable : yes
/// inplace : yes
/// Compare :
/// Swap :
/// Order : O(n^2)
/// </remarks>
/// <typeparam name="T"></typeparam>
public class GnomeSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Exchange;
    protected override string Name => nameof(GnomeSort<T>);

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
        for (var i = 0; i < span.Length; i++)
        {
            while (i > 0 && Compare(Index(span, i - 1), Index(span, i)) > 0)
            {
                Swap(ref Index(span, i - 1), ref Index(span, i));
                i--;
            }
        }
    }
}

/// <summary>
/// 目の前の要素が現在の要素より小さければ前に、大きければ入れ替えて後ろに進む。一番後ろに行ったときは、前に進む。<see cref="InsertionSort{T}"/>に似ているが、挿入ではなく交換となっている。常に直前とのみ比較するので、ソート中に末尾要素が追加されも問題ない。
/// BubbleSortよりは高速でInsertSort程度
/// </summary>
/// <remarks>
/// stable : yes
/// inplace : yes
/// Compare :
/// Swap :
/// Order : O(n^2)
/// </remarks>
/// <typeparam name="T"></typeparam>
public class GnomeSortWithSwap<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Exchange;
    protected override string Name => nameof(GnomeSortWithSwap<T>);

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
        for (var i = 1; i < span.Length;)
        {
            if (Compare(Index(span, i - 1), Index(span, i)) <= 0)
            {
                i++;
            }
            else
            {
                Swap(ref Index(span, i - 1), ref Index(span, i));
                i -= 1;
                if (i == 0) i = 1;
            }
        }
    }
}

/// <summary>
/// <see cref="GnomeSort{T}"/> に似ているが最適化がされていない
/// </summary>
/// <typeparam name="T"></typeparam>
public class GnomeSortNoOptimization<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Exchange;
    protected override string Name => nameof(GnomeSortNoOptimization<T>);

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
        for (var i = 0; i < span.Length - 1; i++)
        {
            if (Compare(Index(span, i), Index(span, i + 1)) > 0)
            {
                Swap(ref Index(span, i), ref Index(span, i + 1));
                for (var j = i; j > 0; j--)
                {
                    if (Compare(Index(span, j - 1), Index(span, j)) <= 0) break;

                    Swap(ref Index(span, j - 1), ref Index(span, j));
                }
            }
        }
    }
}

/// <summary>
/// 実装はシンプルだが遅い
/// </summary>
/// <typeparam name="T"></typeparam>
public class GnomeSortSimple<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Exchange;
    protected override string Name => nameof(GnomeSortSimple<T>);

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
        var i = 0;
        while (i < span.Length)
        {
            if (i == 0 || Compare(Index(span, i - 1), Index(span, i)) <= 0)
            {
                i++;
            }
            else
            {
                Swap(ref Index(span, i), ref Index(span, i - 1));
                --i;
            }
        }
    }
}
