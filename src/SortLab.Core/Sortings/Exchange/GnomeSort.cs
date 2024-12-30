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
    public override SortType SortType => SortType.Exchange;
    protected override string Name => nameof(GnomeSort<T>);

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        for (var i = 0; i < array.Length; i++)
        {
            Statistics.AddIndexCount();
            while (i > 0 && Compare(array[i - 1], array[i]) > 0)
            {
                Swap(ref array[i - 1], ref array[i]);
                i--;
            }
        }
        return array;
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
    public override SortType SortType => SortType.Exchange;
    protected override string Name => nameof(GnomeSortWithSwap<T>);

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        for (var i = 1; i < array.Length;)
        {
            Statistics.AddIndexCount();
            if (Compare(array[i - 1], array[i]) <= 0)
            {
                i++;
            }
            else
            {
                Swap(ref array[i - 1], ref array[i]);
                i -= 1;
                if (i == 0) i = 1;
            }

        }
        return array;
    }
}

/// <summary>
/// <see cref="GnomeSort{T}"/> に似ているが最適化がされていない
/// </summary>
/// <typeparam name="T"></typeparam>
public class GnomeSortNoOptimization<T> : SortBase<T> where T : IComparable<T>
{
    public override SortType SortType => SortType.Exchange;
    protected override string Name => nameof(GnomeSortNoOptimization<T>);

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        for (var i = 0; i < array.Length - 1; i++)
        {
            if (Compare(array[i], array[i + 1]) > 0)
            {
                Swap(ref array[i], ref array[i + 1]);
                for (var j = i; j > 0; j--)
                {
                    Statistics.AddIndexCount();
                    if (Compare(array[j - 1], array[j]) <= 0) break;

                    Swap(ref array[j - 1], ref array[j]);
                }
            }
            else
            {
                Statistics.AddIndexCount();
            }
        }
        return array;
    }
}

/// <summary>
/// 実装はシンプルだが遅い
/// </summary>
/// <typeparam name="T"></typeparam>
public class GnomeSortSimple<T> : SortBase<T> where T : IComparable<T>
{
    public override SortType SortType => SortType.Exchange;
    protected override string Name => nameof(GnomeSortSimple<T>);

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);

        var i = 0;
        while (i < array.Length)
        {
            Statistics.AddIndexCount();
            if (i == 0 || Compare(array[i - 1], array[i]) <= 0)
            {
                i++;
            }
            else
            {
                Swap(ref array[i], ref array[i - 1]);
                --i;
            }
        }
        return array;
    }
}
