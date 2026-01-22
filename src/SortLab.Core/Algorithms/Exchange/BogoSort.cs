using SortLab.Core.Contexts;

namespace SortLab.Core.Algorithms;

/*

Span ...

| Method   | Number | Mean         | Error          | StdDev       | Median         | Min         | Max           | Allocated |
|--------- |------- |-------------:|---------------:|-------------:|---------------:|------------:|--------------:|----------:|
| BogoSort | 10     |     2.350 ms |      62.034 ms |     3.400 ms |      0.4589 ms |   0.3152 ms |      6.275 ms |     448 B |
| BogoSort | 13     | 9,342.105 ms | 136,583.409 ms | 7,486.598 ms | 13,532.8534 ms | 698.6498 ms | 13,794.812 ms |     736 B |

*/

/// <summary>
/// 配列をランダムにシャッフルし、ソートされているかを確認することを繰り返す、非常に非効率なソートアルゴリズムです。Permutation Sortとも呼ばれます。
/// 10要素程度が現実的な時間で終了する限界です。
/// <br/>
/// Continuously shuffles the array randomly until it is sorted, checking after each shuffle. This approach is extremely inefficient and impractical for sorting.
/// 10 elements is about the limit for completing in a realistic time.
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
public static class BogoSort
{
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
    {
        Sort(span, NullContext.Default);
    }

    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T>(span, context);

        while (!IsSorted(s))
        {
            Shuffle(s);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Shuffle<T>(SortSpan<T> s) where T : IComparable<T>
    {
        // Fisher-Yates shuffle - すべての順列を均等な確率で生成
        var length = s.Length;
        for (var i = length - 1; i > 0; i--)
        {
            s.Swap(i, Random.Shared.Next(0, i + 1));
        }
    }

    private static bool IsSorted<T>(SortSpan<T> s) where T : IComparable<T>
    {
        var length = s.Length;
        for (var i = 0; i < length - 1; i++)
        {
            if (s.Compare(i, i + 1) > 0)
            {
                return false;
            }
        }
        return true;
    }
}
