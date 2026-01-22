using SortLab.Core.Contexts;
using System.Diagnostics;

namespace SortLab.Core.Algorithms;

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
/// <see cref="InsertionSort"/>にギャップ付き挿入ソートの概念を適用したシェルソートアルゴリズム。
/// この例では、一般的だが必須ではない初期の h の制限として 'length / 9' を使用します。
/// h_{i+1} = 3h_i + 1 となる h で配列を分割し、分割された各部分配列ごとに挿入ソート <see cref="InsertionSort"/> を行う。
/// 次の h を /3 で求め、h が 1 になるまでこの操作を繰り返す。各 h ごとに部分配列がソートされているため、最後の h=1 のときは通常の挿入ソートと同じだが、既に部分的にソートされているため高速に動作する。
/// ギャップ付き挿入ソートを使用するため不安定なソートである。（ギャップを使って要素を大きく飛ばしながらソートするため、同値要素の相対順序が保たれない）
/// <see cref="BubbleSort"/> に同様の概念を適用したものが <see cref="CombSort"/> である。
/// <br/>
/// <see cref="InsertionSort"/> with gap concept applied is Shell sort algorithm.
/// In this example, we use 'length / 9' as an initial limit for h, which is common but not mandatory.
/// The array is logically divided according to the gap 'h', and each sub-array is sorted　using insertion-sort-like steps (<see cref="InsertionSort"/>).
/// Then we reduce 'h' by dividing by 3 and repeat this process until h=1. By the time h=1, the data is already partially sorted, so the final pass (which is effectively an insertion sort) is very efficient.
/// This approach is a "gap-based insertion sort," so it is inherently unstable.
/// <see cref="CombSort"/> is a similar concept applied to <see cref="BubbleSort"/>.
/// <br/>
/// Knuth's sequence: h = 3*h + 1
/// Concrete sequence: [1, 4, 13, 40, 121, 364, 1093, 3280, 9841, 29524, 88573, 265720, 797161, 2391484]
/// </summary>
/// <remarks>
/// family  : insertion
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
public static class ShellSortKnuth1973
{
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
    {
        Sort(span, 0, span.Length, NullContext.Default);
    }

    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        Sort(span, 0, span.Length, context);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void Sort<T>(Span<T> span, int first, int last, ISortContext context) where T : IComparable<T>
    {
        var length = last - first;
        Debug.Assert(first >= 0 && last <= span.Length && first <= last, "Invalid range for sorting.");

        if (length < 2) return;

        var s = new SortSpan<T>(span, context);

        // Knuth's sequence: h = 3*h + 1
        ReadOnlySpan<int> knuthSequence = [1, 4, 13, 40, 121, 364, 1093, 3280, 9841, 29524, 88573, 265720, 797161, 2391484];

        // Find the largest gap index where gap <= (length/2)
        int gapIndex = knuthSequence.Length - 1;
        while (gapIndex >= 0 && knuthSequence[gapIndex] > length / 2)
            gapIndex--;

        // Decrease gap by moving to previous index
        for (; gapIndex >= 0; gapIndex--)
        {
            var h = knuthSequence[gapIndex];
            
            // Swap based Insertion sort with gap h.
            for (var i = first + h; i < last; i++)
            {
                // Ensure j >= first + h to stay within the subrange.
                for (int j = i; j >= first + h && s.Compare(j - h, j) > 0; j -= h)
                {
                    s.Swap(j, j - h);
                }
            }
        }
    }
}

/// <summary>
/// <see cref="InsertionSort"/>にギャップ付き挿入ソートの概念を適用したシェルソートアルゴリズム。
/// Sedgewickのシーケンスを使用：h = 4^k + 3*2^(k-1) + 1
/// この例では、一般的だが必須ではない初期の h の制限として 'length / 9' を使用します。
/// ギャップ付き挿入ソートを使用するため不安定なソートである。（ギャップを使って要素を大きく飛ばしながらソートするため、同値要素の相対順序が保たれない）
/// <see cref="BubbleSort"/> に同様の概念を適用したものが <see cref="CombSort"/> である。
/// <br/>
/// <see cref="InsertionSort"/> with gap concept applied is Shell sort algorithm.
/// Shell sort using a typical Sedgewick sequence (h = 4^k + 3*2^(k-1) + 1).
/// Note that Sedgewick also has various formula-based sequences.
/// This approach is a "gap-based insertion sort," so it is inherently unstable.
/// <see cref="CombSort"/> is a similar concept applied to <see cref="BubbleSort"/>.
/// <br/>
/// Concrete sequence: 1, 5, 19, 41, 109, 209, 505, 929, 2161, 3905, ...
/// </summary>
/// <remarks>
/// family  : insertion
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
public static class ShellSortSedgewick1986
{
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
    {
        Sort(span, 0, span.Length, NullContext.Default);
    }

    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        Sort(span, 0, span.Length, context);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void Sort<T>(Span<T> span, int first, int last, ISortContext context) where T : IComparable<T>
    {
        var length = last - first;
        Debug.Assert(first >= 0 && last <= span.Length && first <= last, "Invalid range for sorting.");

        if (length < 2) return;

        var s = new SortSpan<T>(span, context);

        // A partial Sedgewick sequence. Different references may show slightly different numbers.
        ReadOnlySpan<int> sedgewickSequence = [1, 5, 19, 41, 109, 209, 505, 929, 2161, 3905];

        // Find the largest gap index where gap <= (length/2)
        int gapIndex = sedgewickSequence.Length - 1;
        while (gapIndex >= 0 && sedgewickSequence[gapIndex] > length / 2)
            gapIndex--;

        // Decrease gap by moving to previous index
        for (; gapIndex >= 0; gapIndex--)
        {
            var h = sedgewickSequence[gapIndex];

            // Swap based Insertion sort with gap h.
            for (var i = first + h; i < last; i++)
            {
                // Ensure j >= first + h to stay within the subrange.
                for (var j = i; j >= first + h && s.Compare(j - h, j) > 0; j -= h)
                {
                    s.Swap(j, j - h);
                }
            }
        }
    }
}


/// <summary>
/// <see cref="InsertionSort"/>にギャップ付き挿入ソートの概念を適用したシェルソートアルゴリズム。
/// h を (4*h - 1)/9 で減少させるループも使用する。
/// ギャップ付き挿入ソートを使用するため不安定なソートである。（ギャップを使って要素を大きく飛ばしながらソートするため、同値要素の相対順序が保たれない）
/// <see cref="BubbleSort"/> に同様の概念を適用したものが <see cref="CombSort"/> である。
/// <br/>
/// <see cref="InsertionSort"/> with gap concept applied is Shell sort algorithm, with Tokuda (1992) sequence.
/// We also shrink h by (4*h - 1)/9 in the loop.
/// This approach is a "gap-based insertion sort," so it is inherently unstable.
/// <see cref="CombSort"/> is a similar concept applied to <see cref="BubbleSort"/>.
/// <br/>
/// Tokuda's sequence: h_{n+1} = floor((9*h_n + 1)/4)
/// Concrete sequence: [1, 4, 9, 20, 46, 103, 233, 525, 1182, 2660, 5985, 13467, 30301, 68178, 153401, 345152, 776591]
/// </summary>
/// <remarks>
/// family  : insertion
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
public static class ShellSortTokuda1992
{
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
    {
        Sort(span, 0, span.Length, NullContext.Default);
    }

    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        Sort(span, 0, span.Length, context);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void Sort<T>(Span<T> span, int first, int last, ISortContext context) where T : IComparable<T>
    {
        var length = last - first;
        Debug.Assert(first >= 0 && last <= span.Length && first <= last, "Invalid range for sorting.");

        if (length < 2) return;

        var s = new SortSpan<T>(span, context);

        // Tokuda's sequence: empirically optimized gap sequence
        ReadOnlySpan<int> tokudaSequence = [1, 4, 9, 20, 46, 103, 233, 525, 1182, 2660, 5985, 13467, 30301, 68178, 153401, 345152, 776591];

        // Find the largest gap index where gap <= (length/2)
        int gapIndex = tokudaSequence.Length - 1;
        while (gapIndex >= 0 && tokudaSequence[gapIndex] > length / 2)
            gapIndex--;

        // Decrease gap by moving to previous index
        for (; gapIndex >= 0; gapIndex--)
        {
            var h = tokudaSequence[gapIndex];

            // Swap based Insertion sort with gap h.
            for (int i = first + h; i < last; i++)
            {
                // Ensure j >= first + h to stay within the subrange.
                for (int j = i; j >= first + h && s.Compare(j - h, j) > 0; j -= h)
                {
                    s.Swap(j, j - h);
                }
            }
        }
    }
}


/// <summary>
/// <see cref="InsertionSort"/>にギャップ付き挿入ソートの概念を適用したCiura (2001)のシェルソートアルゴリズム。
/// それ以降は h_{k+1} = floor(2.25 * h_k) を使用してシーケンスを拡張できる。
/// シェルソートにおける最もよく知られたギャップシーケンスの1つと考えられている。
/// 参考文献: Marcin Ciura, "Best Increments for the Average Case of Shellsort" (2001)
/// ギャップ付き挿入ソートを使用するため不安定なソートである。（ギャップを使って要素を大きく飛ばしながらソートするため、同値要素の相対順序が保たれない）
/// <see cref="BubbleSort"/> に同様の概念を適用したものが <see cref="CombSort"/> である。
/// <br/>
/// <see cref="InsertionSort"/> with gap concept applied is Shell sort algorithm, with Ciura (2001) sequence.
/// This is considered one of the best-known gap sequences for shell sort.
/// Reference: Marcin Ciura, "Best Increments for the Average Case of Shellsort" (2001)
/// This approach is a "gap-based insertion sort," so it is inherently unstable.
/// <see cref="CombSort"/> is a similar concept applied to <see cref="BubbleSort"/>.
/// <br/>
/// Ciura's sequence: practically optimized gap sequence for first 8 gaps, beyond that, the sequence can be extended using h_{k+1} = floor(2.25 * h_k).
/// Concrete sequence: [1, 4, 10, 23, 57, 132, 301, 701] + [1750, 3937, 8858, 19930, 44844, 100899]
/// </summary>
/// <remarks>
/// family  : insertion
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
public static class ShellSortCiura2001
{
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
    {
        Sort(span, 0, span.Length, NullContext.Default);
    }

    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        Sort(span, 0, span.Length, context);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void Sort<T>(Span<T> span, int first, int last, ISortContext context) where T : IComparable<T>
    {
        var length = last - first;
        Debug.Assert(first >= 0 && last <= span.Length && first <= last, "Invalid range for sorting.");

        if (length < 2) return;

        var s = new SortSpan<T>(span, context);

        // Ciura's empirically determined sequence (extended for larger arrays)
        ReadOnlySpan<int> ciuraSequence = [1, 4, 10, 23, 57, 132, 301, 701, 1750, 3937, 8858, 19930, 44844, 100899];

        // Find the largest gap index where gap <= (length/2)
        int gapIndex = ciuraSequence.Length - 1;
        while (gapIndex >= 0 && ciuraSequence[gapIndex] > length / 2)
            gapIndex--;

        // Decrease gap by moving to previous index
        for (; gapIndex >= 0; gapIndex--)
        {
            var h = ciuraSequence[gapIndex];

            // Swap based Insertion sort with gap h.
            for (var i = first + h; i < last; i++)
            {
                // Ensure j >= first + h to stay within the subrange.
                for (var j = i; j >= first + h && s.Compare(j - h, j) > 0; j -= h)
                {
                    s.Swap(j, j - h);
                }
            }
        }
    }
}

/// <summary>
/// <see cref="InsertionSort"/>にギャップ付き挿入ソートの概念を適用したLee (2021)のシェルソートアルゴリズム。
/// Tokudaシーケンスを改良した最新のシーケンスで、平均比較回数がさらに少ない。
/// ギャップ付き挿入ソートを使用するため不安定なソート。
/// <br/>
/// <see cref="InsertionSort"/> with gap concept applied is Shell sort algorithm, with Lee (2021) sequence.
/// Lee improved upon Tokuda's sequence using γ = 2.243609061420001 with formula h_k = ⌈(γ^k - 1)/(γ - 1)⌉.
/// This sequence: 1, 4, 9, 20, 45, 102, 230, 516, 1158, 2599, 5831, 13082, ...
/// Reference: Ying Wai Lee, "Empirically Improved Tokuda Gap Sequence in Shellsort" (arXiv:2112.11112, 2021)
/// This approach is a "gap-based insertion sort," so it is inherently unstable.
/// <see cref="CombSort"/> is a similar concept applied to <see cref="BubbleSort"/>.
/// <br/>
/// Lee's sequence: h_k = ceil((gamma^k - 1) / (gamma - 1))
/// Concrete sequence: [1, 4, 9, 20, 45, 102, 230, 516, 1158, 2599, 5831, 13082, 29351, 65853, 147748, 331490, 743735]
/// </summary>
/// <remarks>
/// family  : insertion
/// stable  : no  (gap-based insertion sorting does not preserve the order of equal elements)
/// inplace : yes
/// sequence: Lee (2021) - h_k = ⌈(γ^k - 1)/(γ - 1)⌉ where γ = 2.243609061420001
/// Compare : O(n^1.25) (Empirically improved Tokuda, fewer comparisons on average)
/// Swap    : O(n^1.25) (Similar to comparison count)
/// Index   : O(n^1.25) (Each element accessed during gap-based insertion)
/// Order   : Sub-quadratic with state-of-the-art performance
///         * average   : O(n^1.25) (improved over Tokuda)
///         * best case : O(n log n) (nearly sorted)
///         * worst case: O(n^1.5)
/// 
/// Characteristics:
/// - Most recent improvement (2021) of Tokuda's sequence
/// - Empirically yields fewer average comparisons than Tokuda
/// - Uses optimal γ value found through extensive experiments
/// - State-of-the-art gap sequence
/// - Recommended for research and modern implementations
/// </remarks>
/// <typeparam name="T"></typeparam>
public static class ShellSortLee2021
{
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
    {
        Sort(span, 0, span.Length, NullContext.Default);
    }

    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        Sort(span, 0, span.Length, context);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void Sort<T>(Span<T> span, int first, int last, ISortContext context) where T : IComparable<T>
    {
        var length = last - first;
        Debug.Assert(first >= 0 && last <= span.Length && first <= last, "Invalid range for sorting.");

        if (length < 2) return;

        var s = new SortSpan<T>(span, context);

        // Lee's empirically determined sequence based on gamma = 2.243609061420001
        // Formula: h_k = ceil((gamma^k - 1) / (gamma - 1))
        ReadOnlySpan<int> leeSequence = [1, 4, 9, 20, 45, 102, 230, 516, 1158, 2599, 5831, 13082, 29351, 65853, 147748, 331490, 743735];

        // Find the largest gap index where gap <= (length/2)
        int gapIndex = leeSequence.Length - 1;
        while (gapIndex >= 0 && leeSequence[gapIndex] > length / 2)
            gapIndex--;

        // Decrease gap by moving to previous index
        for (; gapIndex >= 0; gapIndex--)
        {
            var h = leeSequence[gapIndex];

            // Swap based Insertion sort with gap h.
            for (var i = first + h; i < last; i++)
            {
                // Ensure j >= first + h to stay within the subrange.
                for (var j = i; j >= first + h && s.Compare(j - h, j) > 0; j -= h)
                {
                    s.Swap(j, j - h);
                }
            }
        }
    }
}

