namespace SortLab.Core.Sortings;

/// <summary>
/// 配列を半分に割り、左、右それぞれを2つペアまで分割しそれぞれをソート(分割統治)。左側でソートしたペアを2->4->8->16.... と順にマージしつつソートする。右側も同様に、ペアをマージしつつソートする。最後に左と右をマージする。これによりソートの範囲を常に抑えることで常に安定しつつ、安定したソートを行うことができる。
/// TopDown
/// </summary>
/// <remarks>
/// stable : yes
/// inplace : no (n)
/// Compare : n log2 n
/// Swap : n log2 n
/// Order : O(n log n) (Worst case : O(n log n))
/// </remarks>
/// <typeparam name="T"></typeparam>

public class MergeSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Merging;
    protected override string Name => nameof(MergeSort<T>);

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

    private void SortCore(Span<T> span, int left, int right)
    {
        if (left >= right) return;

        var mid = (left + right) / 2;
        var work = new T[(span.Length) / 2];

        // left : merge + sort
        SortCore(span, left, mid);
        // right : merge + sort
        SortCore(span, mid + 1, right);
        // left + right: merge
        Merge(span, left, right, mid, work);
    }

    private void Merge(Span<T> span, int left, int right, int mid, T[] work)
    {
        var max = default(T)!;
        // Copy left part to work
        for (var i = left; i <= mid; i++)
        {
            work[i - left] = Index(span, i);

            // max assign
            if (i - left >= work.Length - 1)
            {
                max = span.ToArray().Max()!;
                break;
            }
        }

        var l = left;
        var r = mid + 1;

        // merge span-left and work
        while (true)
        {
            var k = l + r - (mid + 1);
            // if left is sorted then merge done.
            if (l > mid) break;

            // if right is sorted, do left(work)
            if (r > right)
            {
                while (l <= mid)
                {
                    k = l + r - (mid + 1);
                    Swap(ref Index(span, k), ref work[l - left]);

                    // max assign on edge case
                    if (l - left >= work.Length - 1)
                    {
                        Index(span, right) = max;
                        break;
                    }
                    l++;
                }
                break;
            }

            // sort
            if (Compare(work[l - left], Index(span, r)) < 0)
            {
                Swap(ref Index(span, k), ref work[l - left]);
                l++;
            }
            else
            {
                Swap(ref Index(span, k), ref Index(span, r));
                r++;
            }
        }
    }
}

/// <summary>
/// 非効率なマージソート
/// </summary>
/// <remarks>
/// </remarks>
/// <typeparam name="T"></typeparam>
public class MergeSort2<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Merging;
    protected override string Name => nameof(MergeSort2<T>);

    public override void Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        var result = SortCore(array.AsSpan());
        result.CopyTo(array);
    }

    public override void Sort(Span<T> span)
    {
        Statistics.Reset(span.Length, SortType, Name);
        var result = SortCore(span);
        result.CopyTo(span);
    }

    private Span<T> SortCore(Span<T> span)
    {
        if (span.Length <= 1) return span;

        var mid = span.Length / 2;
        var leftArray = span.Slice(0, mid).ToArray();
        var rightArray = span.Slice(mid).ToArray();
        var leftResult = SortCore(leftArray.AsSpan());
        var rightResult = SortCore(rightArray.AsSpan());
        var result = Merge(leftResult, rightResult);
        return result;
    }

    private Span<T> Merge(Span<T> left, Span<T> right)
    {
        var result = new T[left.Length + right.Length];
        var resultSpan = result.AsSpan();
        var i = 0;
        var j = 0;
        var current = 0;
        while (i < left.Length || j < right.Length)
        {
            if (i < left.Length && j < right.Length)
            {
                if (Compare(left[i], right[j]) <= 0)
                {
                    Index(resultSpan, current) = left[i++];
                }
                else
                {
                    Index(resultSpan, current) = right[j++];
                }
            }
            else if (i < left.Length)
            {
                Index(resultSpan, current) = left[i++];
            }
            else
            {
                Index(resultSpan, current) = right[j++];
            }
            current++;
        }

        return resultSpan;
    }
}
