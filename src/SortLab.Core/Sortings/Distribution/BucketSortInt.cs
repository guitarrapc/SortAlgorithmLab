namespace SortLab.Core.Sortings;

/// <summary>
/// 必要となるバケットをはじめに用意して、配列の各要素をバケットに詰める。最後にバケットの中身を結合すればソートが済んでいる。比較を一切しない安定な外部ソート。
/// 問題は、桁の数だけバケットの数が膨大になること
/// </summary>
/// <remarks>
/// stable : yes
/// inplace : no (n*k)
/// Compare : 2n
/// Swap : 0
/// Order : O(n) (Worst case : O(n^2))
/// </remarks>
/// <typeparam name="T"></typeparam>

public class BucketSort<T>(Func<T, int> getKey) : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Distributed;
    protected override string Name => nameof(BucketSort<T>);

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
        if (span.Length <= 1) return;

        // Calculate size and offset
        var min = int.MaxValue;
        var max = int.MinValue;

        for (var i = 0; i < span.Length; i++)
        {
            var key = getKey(Index(span, i));
            if (key < min) min = key;
            if (key > max) max = key;
        }

        var offset = min < 0 ? Math.Abs(min) : 0;
        var size = max - min + 1;

        var bucket = new List<T>[size];

        // Fill buckets
        for (var i = 0; i < span.Length; i++)
        {
            var item = Index(span, i);
            var key = getKey(item) + offset;
            bucket[key] ??= new List<T>();
            bucket[key].Add(item);
        }

        // Write back to span
        for (int j = 0, i = 0; j < bucket.Length; j++)
        {
            if (bucket[j] != null)
            {
                foreach (var item in bucket[j])
                {
                    Index(span, i++) = item;
                }
            }
        }
    }
}

/// <summary>
/// 整数専用のバケットソート
/// </summary>
/// <remarks>
/// </remarks>
/// <typeparam name="T"></typeparam>
public class BucketSortInt<T> : SortBase<int>
{
    public override SortMethod SortType => SortMethod.Distributed;
    protected override string Name => nameof(BucketSortInt<T>);

    public override void Sort(int[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        SortCore(array.AsSpan());
    }

    public override void Sort(Span<int> span)
    {
        Statistics.Reset(span.Length, SortType, Name);
        SortCore(span);
    }

    private void SortCore(Span<int> span)
    {
        if (span.Length <= 1) return;

        // Find min and max
        var min = int.MaxValue;
        var max = int.MinValue;

        for (var i = 0; i < span.Length; i++)
        {
            var value = Index(span, i);
            if (value < min) min = value;
            if (value > max) max = value;
        }

        var offset = min < 0 ? Math.Abs(min) : 0;
        var size = max - min + 1;

        // Make bucket for possibly assigned number of int
        var bucket = new int[size];
        for (var i = 0; i < span.Length; i++)
        {
            bucket[Index(span, i) + offset]++;
        }

        // Put array int to each bucket
        for (int j = 0, i = 0; j < bucket.Length; j++)
        {
            for (var k = bucket[j]; k != 0; k--, i++)
            {
                Index(span, i) = j - offset;
            }
        }
    }
}
