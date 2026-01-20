namespace SortLab.Core.Sortings;

/*

Span ...

| Method   | Number | Mean         | Error          | StdDev       | Median         | Min         | Max           | Allocated |
|--------- |------- |-------------:|---------------:|-------------:|---------------:|------------:|--------------:|----------:|
| BogoSort | 10     |     2.350 ms |      62.034 ms |     3.400 ms |      0.4589 ms |   0.3152 ms |      6.275 ms |     448 B |
| BogoSort | 13     | 9,342.105 ms | 136,583.409 ms | 7,486.598 ms | 13,532.8534 ms | 698.6498 ms | 13,794.812 ms |     736 B |

*/

/// <summary>
/// 配列をランダムにシャッフルし、ソートされているかを確認することを繰り返す、非常に非効率なソートアルゴリズムです。Permutation Sortとも呼ばれます。10ソートで事実上限界<br/>
/// Continuously shuffles the array randomly until it is sorted, checking after each shuffle. This approach is extremely inefficient and impractical for sorting.
/// </summary>
/// <remarks>
/// stable  : no
/// inplace : yes
/// Compare : -    (Comparison operations are performed to check if the array is sorted, but their count is not fixed)  
/// Swap    : -    (Shuffle operations perform swaps or random permutations of the array elements)  
/// Index   : -    (Access frequency depends on the implementation of shuffle and sorted-checking routines)  
/// Order   : O((n+1)!) on average (Worst case: unbounded runtime)
/// </remarks>
/// <typeparam name="T"></typeparam>
public class BogoSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Exchange;
    protected override string Name => nameof(BogoSort<T>);

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
        while (!IsSorted(span))
        {
            Shuffle(span);
        }
    }

    private void Shuffle(Span<T> span)
    {
        var length = span.Length;
        for (var i = 0; i < length; i++)
        {
            Swap(ref Index(span, i), ref Index(span, Random.Shared.Next(0, length)));
        }
    }

    private bool IsSorted(Span<T> span)
    {
        var length = span.Length;
        for (var i = 0; i < length - 1; i++)
        {
            if (Compare(Index(span, i), Index(span, i + 1)) > 0)
            {
                return false;
            }
        }
        return true;
    }
}
