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
/// シェルソートアルゴリズム - ギャップベースの比較を使用した改良版挿入ソート。
/// 配列を「ギャップ」で区切られたサブ配列に分割し、各サブ配列を挿入ソートでソートします。
/// ギャップが1に減少すると配列はほぼソート済みになり、最終的な挿入ソートパスが非常に効率的になります。
/// <br/>
/// この実装はKnuthのギャップシーケンス（1973）を使用: h = 3h + 1
/// ギャップシーケンス: 1, 4, 13, 40, 121, 364, 1093, 3280, 9841, ...
/// 最も有名で広く使われているギャップシーケンス。
/// <br/>
/// Shell sort algorithm - an improved insertion sort using gap-based comparisons.
/// Shell sort divides the array into sub-arrays separated by a "gap" and sorts each sub-array using insertion sort.
/// As the gap reduces to 1, the array becomes nearly sorted, making the final insertion sort pass very efficient.
/// <br/>
/// This implementation uses Knuth's gap sequence (1973): h = 3h + 1
/// Gap sequence: 1, 4, 13, 40, 121, 364, 1093, 3280, 9841, ...
/// This is the most well-known and widely used gap sequence for shell sort.
/// </summary>
/// <remarks>
/// family  : insertion
/// stable  : no  (gap-based insertion sorting does not preserve the order of equal elements)
/// inplace : yes
/// Compare : O(n^1.5) ~ O(n^1.25) (Knuth sequence has well-studied complexity)
/// Swap    : O(n^1.5) ~ O(n^1.25) (Similar to comparison count)
/// Index   : O(n^1.5) (Each element accessed during gap-based insertion)
/// Order   : Sub-quadratic
///         * average   : O(n^1.5) ~ O(n^1.25)
///         * best case : O(n log n) (nearly sorted)
///         * worst case: O(n^1.5)
/// </remarks>
/// <typeparam name="T"></typeparam>
public static class ShellSortKnuth1973
{
    // Characteristics:
    // - Most famous and classic gap sequence
    // - Easy to implement with simple formula
    // - Good general-purpose performance
    // - Well-studied theoretical properties

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
/// シェルソートアルゴリズム - ギャップベースの比較を使用した改良版挿入ソート。
/// 配列を「ギャップ」で区切られたサブ配列に分割し、各サブ配列を挿入ソートでソートします。
/// ギャップが1に減少すると配列はほぼソート済みになり、最終的な挿入ソートパスが非常に効率的になります。
/// <br/>
/// この実装はSedgewickのギャップシーケンス（1986）を使用: h_k = 4^k + 3·2^(k-1) + 1
/// ギャップシーケンス: 1, 5, 19, 41, 109, 209, 505, 929, 2161, 3905, ...
/// 理論的に優れた特性を持つ数式ベースのシーケンス。
/// <br/>
/// Shell sort algorithm - an improved insertion sort using gap-based comparisons.
/// Shell sort divides the array into sub-arrays separated by a "gap" and sorts each sub-array using insertion sort.
/// As the gap reduces to 1, the array becomes nearly sorted, making the final insertion sort pass very efficient.
/// <br/>
/// This implementation uses Sedgewick's gap sequence (1986): h_k = 4^k + 3·2^(k-1) + 1
/// Gap sequence: 1, 5, 19, 41, 109, 209, 505, 929, 2161, 3905, ...
/// This formula-based sequence has better theoretical complexity bounds than Knuth.
/// </summary>
/// <remarks>
/// family  : insertion
/// stable  : no  (gap-based insertion sorting does not preserve the order of equal elements)
/// inplace : yes
/// Compare : O(n^1.33) ~ O(n^1.25) (Sedgewick sequence has good theoretical bounds)
/// Swap    : O(n^1.33) ~ O(n^1.25) (Similar to comparison count)
/// Index   : O(n^1.33) (Each element accessed during gap-based insertion)
/// Order   : Sub-quadratic with good theoretical properties
///         * average   : O(n^1.33) ~ O(n^1.25)
///         * best case : O(n log n) (nearly sorted)
///         * worst case: O(n^1.33)
/// </remarks>
/// <typeparam name="T"></typeparam>
public static class ShellSortSedgewick1986
{
    // Characteristics:
    // - Formula-based gap sequence
    // - Good theoretical complexity bounds
    // - Better than Knuth in many cases
    // - Predictable performance characteristics

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
/// シェルソートアルゴリズム - ギャップベースの比較を使用した改良版挿入ソート。
/// 配列を「ギャップ」で区切られたサブ配列に分割し、各サブ配列を挿入ソートでソートします。
/// ギャップが1に減少すると配列はほぼソート済みになり、最終的な挿入ソートパスが非常に効率的になります。
/// <br/>
/// この実装はTokudaのギャップシーケンス（1992）を使用: h_k = ⌈(9/4)^k⌉ or h_{k+1} = ⌊(9h_k + 1)/4⌋
/// ギャップシーケンス: 1, 4, 9, 20, 46, 103, 233, 525, 1182, 2660, 5985, ...
/// 実験的に最適化されたシーケンスで、優れた実用性能を持つ。
/// <br/>
/// Shell sort algorithm - an improved insertion sort using gap-based comparisons.
/// Shell sort divides the array into sub-arrays separated by a "gap" and sorts each sub-array using insertion sort.
/// As the gap reduces to 1, the array becomes nearly sorted, making the final insertion sort pass very efficient.
/// <br/>
/// This implementation uses Tokuda's gap sequence (1992): h_k = ⌈(9/4)^k⌉ or h_{k+1} = ⌊(9h_k + 1)/4⌋
/// Gap sequence: 1, 4, 9, 20, 46, 103, 233, 525, 1182, 2660, 5985, ...
/// This empirically optimized sequence offers better practical performance than Knuth.
/// </summary>
/// <remarks>
/// family  : insertion
/// stable  : no  (gap-based insertion sorting does not preserve the order of equal elements)
/// inplace : yes
/// Compare : O(n^1.25) (Empirically optimized, better than Knuth)
/// Swap    : O(n^1.25) (Similar to comparison count)
/// Index   : O(n^1.25) (Each element accessed during gap-based insertion)
/// Order   : Sub-quadratic with excellent practical performance
///         * average   : O(n^1.25)
///         * best case : O(n log n) (nearly sorted)
///         * worst case: O(n^1.5)
/// </remarks>
/// <typeparam name="T"></typeparam>
public static class ShellSortTokuda1992
{
    // Characteristics:
    // - Empirically optimized gap sequence
    // - Better practical performance than Knuth
    // - Good balance between theory and practice
    // - Widely used in production systems

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
/// シェルソートアルゴリズム - ギャップベースの比較を使用した改良版挿入ソート。
/// 配列を「ギャップ」で区切られたサブ配列に分割し、各サブ配列を挿入ソートでソートします。
/// ギャップが1に減少すると配列はほぼソート済みになり、最終的な挿入ソートパスが非常に効率的になります。
/// <br/>
/// この実装はCiuraのギャップシーケンス（2001）を使用: 実験的に決定された最適ギャップ
/// ギャップシーケンス: 1, 4, 10, 23, 57, 132, 301, 701, 1750, ... (h_{k+1} = ⌊2.25h_k⌋で拡張)
/// 平均ケース性能において最良のギャップシーケンスの一つ。
/// <br/>
/// Shell sort algorithm - an improved insertion sort using gap-based comparisons.
/// Shell sort divides the array into sub-arrays separated by a "gap" and sorts each sub-array using insertion sort.
/// As the gap reduces to 1, the array becomes nearly sorted, making the final insertion sort pass very efficient.
/// <br/>
/// This implementation uses Ciura's gap sequence (2001): empirically determined optimal gaps
/// Gap sequence: 1, 4, 10, 23, 57, 132, 301, 701, 1750, ... (extended by h_{k+1} = ⌊2.25h_k⌋)
/// This is widely considered one of the best gap sequences for average-case performance.
/// </summary>
/// <remarks>
/// family  : insertion
/// stable  : no  (gap-based insertion sorting does not preserve the order of equal elements)
/// inplace : yes
/// Compare : O(n^1.3) (One of the best known gap sequences for average case)
/// Swap    : O(n^1.3) (Similar to comparison count)
/// Index   : O(n^1.3) (Each element accessed during gap-based insertion)
/// Order   : Sub-quadratic with best known practical performance
///         * average   : O(n^1.3) (empirically best)
///         * best case : O(n log n) (nearly sorted)
///         * worst case: O(n^1.5)
/// </remarks>
/// <typeparam name="T"></typeparam>
public static class ShellSortCiura2001
{
    // Characteristics:
    // - Experimentally optimized for best average-case performance
    // - Considered one of the best gap sequences known
    // - First 8 gaps are empirically determined (1, 4, 10, 23, 57, 132, 301, 701)
    // - Extended gaps use formula h_{k+1} = ⌊2.25h_k⌋
    // - Recommended for general-purpose use

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
/// シェルソートアルゴリズム - ギャップベースの比較を使用した改良版挿入ソート。
/// 配列を「ギャップ」で区切られたサブ配列に分割し、各サブ配列を挿入ソートでソートします。
/// ギャップが1に減少すると配列はほぼソート済みになり、最終的な挿入ソートパスが非常に効率的になります。
/// <br/>
/// この実装はLeeのギャップシーケンス（2021）を使用: h_k = ⌈(γ^k - 1)/(γ - 1)⌉ (γ = 2.243609061420001)
/// ギャップシーケンス: 1, 4, 9, 20, 45, 102, 230, 516, 1158, 2599, 5831, 13082, ...
/// Tokudaシーケンスの改良版で、平均比較回数がより少ない。
/// <br/>
/// Shell sort algorithm - an improved insertion sort using gap-based comparisons.
/// Shell sort divides the array into sub-arrays separated by a "gap" and sorts each sub-array using insertion sort.
/// As the gap reduces to 1, the array becomes nearly sorted, making the final insertion sort pass very efficient.
/// <br/>
/// This implementation uses Lee's gap sequence (2021): h_k = ⌈(γ^k - 1)/(γ - 1)⌉ where γ = 2.243609061420001
/// Gap sequence: 1, 4, 9, 20, 45, 102, 230, 516, 1158, 2599, 5831, 13082, ...
/// This is an improved version of Tokuda's sequence with fewer average comparisons.
/// Reference: Ying Wai Lee, "Empirically Improved Tokuda Gap Sequence in Shellsort" (arXiv:2112.11112, 2021)
/// </summary>
/// <remarks>
/// family  : insertion
/// stable  : no  (gap-based insertion sorting does not preserve the order of equal elements)
/// inplace : yes
/// Compare : O(n^1.25) (Empirically improved Tokuda, fewer comparisons on average)
/// Swap    : O(n^1.25) (Similar to comparison count)
/// Index   : O(n^1.25) (Each element accessed during gap-based insertion)
/// Order   : Sub-quadratic with state-of-the-art performance
///         * average   : O(n^1.25) (improved over Tokuda)
///         * best case : O(n log n) (nearly sorted)
///         * worst case: O(n^1.5)
/// </remarks>
/// <typeparam name="T"></typeparam>
public static class ShellSortLee2021
{
    // Characteristics:
    // - Most recent improvement (2021) of Tokuda's sequence
    // - Empirically yields fewer average comparisons than Tokuda
    // - Uses optimal γ value found through extensive experiments
    // - State-of-the-art gap sequence
    // - Recommended for research and modern implementations

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

