using System.Runtime.CompilerServices;

namespace SortLab.Core.Sortings;

/*

Ref span (Knuth) ...

| Method           | Number | Mean          | Error        | StdDev      | Median        | Min           | Max           | Allocated |
|----------------- |------- |--------------:|-------------:|------------:|--------------:|--------------:|--------------:|----------:|
| ShellSort        | 100    |      9.767 us |    10.374 us |   0.5686 us |      9.600 us |      9.300 us |     10.400 us |     448 B |
| ShellSort        | 1000   |     42.933 us |     4.591 us |   0.2517 us |     42.900 us |     42.700 us |     43.200 us |     736 B |
| ShellSort        | 10000  |    502.667 us |   142.001 us |   7.7835 us |    503.500 us |    494.500 us |    510.000 us |     736 B |

Ref span (Tokuda) ...

| Method           | Number | Mean          | Error        | StdDev      | Median        | Min           | Max          | Allocated |
|----------------- |------- |--------------:|-------------:|------------:|--------------:|--------------:|-------------:|----------:|
| ShellSort        | 100    |      9.950 us |    15.800 us |   0.8660 us |     10.450 us |      8.950 us |     10.45 us |     448 B |
| ShellSort        | 1000   |     50.400 us |   143.779 us |   7.8810 us |     45.900 us |     45.800 us |     59.50 us |     448 B |
| ShellSort        | 10000  |    564.900 us |    33.441 us |   1.8330 us |    565.300 us |    562.900 us |    566.50 us |     736 B |

Ref span (Sedgewick) ...

| Method           | Number | Mean          | Error        | StdDev      | Median        | Min           | Max           | Allocated |
|----------------- |------- |--------------:|-------------:|------------:|--------------:|--------------:|--------------:|----------:|
| ShellSort        | 100    |     10.333 us |     1.053 us |   0.0577 us |     10.300 us |     10.300 us |     10.400 us |     448 B |
| ShellSort        | 1000   |     46.200 us |    71.151 us |   3.9000 us |     44.100 us |     43.800 us |     50.700 us |     400 B |
| ShellSort        | 10000  |    554.200 us |   179.040 us |   9.8138 us |    553.600 us |    544.700 us |    564.300 us |     736 B |

Span (Knuth) ...

| Method                 | Number | Mean          | Error        | StdDev      | Median        | Min           | Max          | Allocated |
|----------------------- |------- |--------------:|-------------:|------------:|--------------:|--------------:|-------------:|----------:|
| ShellSort              | 100    |     13.300 us |    75.199 us |   4.1219 us |     11.600 us |     10.300 us |     18.00 us |     448 B |
| ShellSort              | 1000   |     52.283 us |   107.813 us |   5.9096 us |     53.250 us |     45.950 us |     57.65 us |     736 B |
| ShellSort              | 10000  |    522.533 us |   258.098 us |  14.1472 us |    515.700 us |    513.100 us |    538.80 us |     448 B |

Span (Tokuda) ...

| Method                 | Number | Mean          | Error        | StdDev      | Median        | Min           | Max           | Allocated |
|----------------------- |------- |--------------:|-------------:|------------:|--------------:|--------------:|--------------:|----------:|
| ShellSort              | 100    |     11.300 us |     6.578 us |   0.3606 us |     11.400 us |     10.900 us |     11.600 us |     736 B |
| ShellSort              | 1000   |     46.367 us |     7.373 us |   0.4041 us |     46.300 us |     46.000 us |     46.800 us |     448 B |
| ShellSort              | 10000  |    540.533 us |   114.955 us |   6.3011 us |    538.500 us |    535.500 us |    547.600 us |     448 B |

Span (Sedgewick) ...

| Method                 | Number | Mean          | Error        | StdDev      | Median        | Min           | Max           | Allocated |
| ShellSort              | 100    |     11.933 us |     3.798 us |   0.2082 us |     12.000 us |     11.700 us |     12.100 us |     448 B |
| ShellSort              | 1000   |     43.500 us |     6.578 us |   0.3606 us |     43.400 us |     43.200 us |     43.900 us |     736 B |
| ShellSort              | 10000  |    557.967 us |   110.752 us |   6.0707 us |    556.900 us |    552.500 us |    564.500 us |     736 B |

 */

/// <summary>
/// h_{i+1} = 3h_i + 1 となる h で配列を分割し、分割された各部分配列ごとに挿入ソート<see cref="InsertionSort{T}"/>を行う。
/// 次の h を /3 で求め、h が 1 になるまでこの操作を繰り返す。各 h ごとに部分配列がソートされているため、最後の h=1 のときは通常の挿入ソートと同じだが、既に部分的にソートされているため高速に動作する。
/// ギャップ付き挿入ソートを使用するため不安定なソートである。（ギャップを使って要素を大きく飛ばしながらソートするため、同値要素の相対順序が保たれない）
/// <see cref="BubbleSort{T}"/> に同様の概念を適用したものが <see cref="CombSort{T}"/> である。
/// <br/>
/// Shell sort algorithm that uses a gap sequence defined by h_{i+1} = 3*h_i + 1.
/// The array is logically divided according to the gap 'h', and each sub-array is sorted　using insertion-sort-like steps (<see cref="InsertionSort{T}"/>). Then we reduce 'h' by dividing by 3 and repeat this process until h=1. By the time h=1, the data is already partially sorted, so the final pass (which is effectively an insertion sort) is very efficient.
/// This approach is a "gap-based insertion sort," so it is inherently unstable.
/// <see cref="CombSort{T}"/> is a similar concept applied to <see cref="BubbleSort{T}"/>.
/// </summary>
/// <remarks>
/// stable  : no  (gap-based insertion sorting does not preserve the order of equal elements)
/// inplace : yes
/// Compare : Depends on gap sequence (often around O(n^1.3) ~ O(n^1.5))
/// Swap    : O(n^1.3) ~ O(n^2) (Potentially multiple swaps per insertion)
/// Index   : O(n^2)   (Each element may be accessed multiple times during swaps) 
/// Order   : Typically sub-quadratic
///         * average   : O(n^1.3 ~ n^1.5)
///         * best case : O(n) (nearly sorted)
///         * worst case: O(n^2)
/// </remarks>
/// <typeparam name="T"></typeparam>
public class ShellSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Insertion;
    protected override string Name => nameof(ShellSort<T>);

    public override void Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        SortCore(array.AsSpan(), 0, array.Length, GapType.Knuth);
    }

    public override void Sort(Span<T> span)
    {
        Statistics.Reset(span.Length, SortType, Name);
        SortCore(span, 0, span.Length, GapType.Knuth);
    }

    /// <summary>
    /// Main entry to switch gap sequences (Knuth, Tokuda, Sedgewick)
    /// </summary>
    /// <param name="span"></param>
    /// <param name="first"></param>
    /// <param name="last"></param>
    /// <param name="gapType"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void SortCore(Span<T> span, int first, int last, GapType gapType)
    {
        switch (gapType)
        {
            case GapType.Knuth:
                {
                    SortCoreKnuth(span, first, last);
                    break;
                }
            case GapType.Tokuda:
                {
                    SortCoreTokuda(span, first, last);
                    break;
                }
            case GapType.Sedgewick:
                {
                    SortCoreSedgewick(span, first, last);
                    break;
                }
            default:
                throw new NotImplementedException(gapType.ToString());
        };
    }

    /// <summary>
    /// Shell sort using the Knuth sequence: h = 3*h + 1 (e.g. 1, 4, 13, 40, 121, ...).
    /// In this example, we use 'length / 9' as an initial limit for h, which is common but not mandatory.
    /// </summary>
    /// <param name="span"></param>
    /// <param name="first"></param>
    /// <param name="last"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SortCoreKnuth(Span<T> span, int first, int last)
    {
        var length = last - first;
        // Only 1 or 0 elements
        if (length < 2)
            return;

        // Calculate the initial gap (Knuth sequence).
        // We stop when h >= length/9. This is just one approach.
        // 1, 4, 13, 40, 121, 364, 1093, ...
        var h = 0;
        while (h < length / 9)
        {
            h = h * 3 + 1;
        }

        // Decrease h by dividing by 3 each iteration until h == 0.
        for (; h > 0; h /= 3)
        {
            // Swap based Insertion sort with gap h.
            for (var i = first + h; i < last; i++)
            {
                // Ensure j >= first + h to stay within the subrange.
                for (int j = i; j >= first + h && Compare(Index(span, j - h), Index(span, j)) > 0; j -= h)
                {
                    Swap(ref Index(span, j), ref Index(span, j - h));
                }
            }
        }
    }

    /// <summary>
    /// Shell sort using the Tokuda sequence: h_{n+1} = floor((9*h_n + 1)/4).
    /// We also shrink h by (4*h - 1)/9 in the loop.
    /// </summary>
    /// <param name="span"></param>
    /// <param name="first"></param>
    /// <param name="last"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SortCoreTokuda(Span<T> span, int first, int last)
    {
        var length = last - first;
        // Only 1 or 0 elements
        if (length < 2)
            return;

        // Initial gap for Tokuda sequence.
        int h = 1;
        while (h < length / 5)
        {
            h = (9 * h + 1) / 4;
        }

        // Decrease gap until it goes to 0.
        while (h > 0)
        {
            // Swap based Insertion sort with gap h.
            for (int i = first + h; i < last; i++)
            {
                // Ensure j >= first + h to stay within the subrange.
                for (int j = i; j >= first + h && Compare(Index(span, j - h), Index(span, j)) > 0; j -= h)
                {
                    Swap(ref Index(span, j), ref Index(span, j - h));
                }
            }

            // Decrease h via (4*h - 1)/9. Make sure it doesn't go negative.
            h = (4 * h - 1) / 9;
            if (h < 1)
                h = 0;
        }
    }

    /// <summary>
    /// Shell sort using a typical Sedgewick sequence (h = 4^k + 3*2^(k-1) + 1).
    /// Note that Sedgewick also has various formula-based sequences; 1, 5, 19, 41, 109, 209, 505, 929, ... is a commonly used subset.
    /// </summary>
    /// <param name="span"></param>
    /// <param name="first"></param>
    /// <param name="last"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SortCoreSedgewick(Span<T> span, int first, int last)
    {
        var length = last - first;
        // Only 1 or 0 elements
        if (length < 2)
            return;

        // A partial Sedgewick sequence. Different references may show slightly different numbers.
        Span<int> sedgewickSequence = [1, 5, 19, 41, 109, 209, 505, 929, 2161, 3905];

        // Find the largest gap <= (length/2)
        var h = 1;
        for (int i = 0; i < sedgewickSequence.Length; i++)
        {
            if (sedgewickSequence[i] > length / 2)
                break;
            h = sedgewickSequence[i];
        }

        // Decrease gap by going to the previous step in the Sedgewick array.
        for (; h > 0; h = GetPreviousSedgewickGap(h))
        {
            // Swap based Insertion sort with gap h.
            for (var i = first + h; i < last; i++)
            {
                // Ensure j >= first + h to stay within the subrange.
                for (var j = i; j >= first + h && Compare(Index(span, j - h), Index(span, j)) > 0; j -= h)
                {
                    Swap(ref Index(span, j), ref Index(span, j - h));
                }
            }
        }
    }

    /// <summary>
    /// Finds the next smaller gap in the Sedgewick sequence by searching backward.
    /// If 'current' is not found in the array, we simply return 0 as a fallback.
    /// </summary>
    private static int GetPreviousSedgewickGap(int current)
    {
        // A partial Sedgewick sequence. Different references may show slightly different numbers. (Should be same as SortCoreSedgewick)
        Span<int> sedgewickSequence = [1, 5, 19, 41, 109, 209, 505, 929, 2161, 3905];

        // If current is not in array, return 0
        int prev = 0;
        for (int i = 0; i < sedgewickSequence.Length; i++)
        {
            if (sedgewickSequence[i] == current)
            {
                // i-1 exists
                if (i > 0)
                {
                    prev = sedgewickSequence[i - 1];
                }
                break;
            }
            else if (sedgewickSequence[i] > current)
            {
                // 'current' might be outside this sequence subset.
                break;
            }
            else
            {
                prev = sedgewickSequence[i];
            }
        }
        return prev;
    }

    private enum GapType
    {
        Knuth,
        Tokuda,
        Sedgewick,
    }
}
