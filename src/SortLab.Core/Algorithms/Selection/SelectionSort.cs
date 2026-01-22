using SortLab.Core.Contexts;
using System.Diagnostics;

namespace SortLab.Core.Algorithms;

/*

Ref span ...

| Method        | Number | Mean          | Error          | StdDev        | Median       | Min          | Max           | Allocated |
|-------------- |------- |--------------:|---------------:|--------------:|-------------:|-------------:|--------------:|----------:|
| SelectionSort | 100    |      15.83 us |      12.147 us |      0.666 us |     16.00 us |     15.10 us |      16.40 us |     736 B |
| SelectionSort | 1000   |     260.53 us |      28.400 us |      1.557 us |    260.70 us |    258.90 us |     262.00 us |     736 B |
| SelectionSort | 10000  |  19,651.13 us |   1,740.622 us |     95.409 us | 19,596.70 us | 19,595.40 us |  19,761.30 us |     448 B |

Span ...

| Method        | Number | Mean          | Error          | StdDev       | Median       | Min          | Max           | Allocated |
|-------------- |------- |--------------:|---------------:|-------------:|-------------:|-------------:|--------------:|----------:|
| SelectionSort | 100    |      17.40 us |       3.649 us |     0.200 us |     17.40 us |     17.20 us |      17.60 us |     448 B |
| SelectionSort | 1000   |     234.67 us |      15.191 us |     0.833 us |    234.40 us |    234.00 us |     235.60 us |     448 B |
| SelectionSort | 10000  |  20,021.47 us |     846.496 us |    46.399 us | 20,012.20 us | 19,980.40 us |  20,071.80 us |     736 B |

 */

/// <summary>
/// 配列を境界で2つの部分（ソート済み部分と未ソート部分）に分割し、未ソート部分から最小要素を見つけて境界位置と交換します。
/// この操作を境界を進めながら繰り返すことでソートを完了します。インデックスベースの交換により不安定なソートアルゴリズムです。
/// <br/>
/// Divides the array into two parts (sorted and unsorted) at a boundary, finds the minimum element in the unsorted portion,
/// and swaps it with the element at the boundary position. Repeats this operation while advancing the boundary to complete sorting.
/// Index-based swapping makes this an unstable sorting algorithm.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Selection Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Partition Invariant:</strong> Maintain two regions in the array: a sorted prefix [0..i) and an unsorted suffix [i..n).
/// After iteration k, the first k elements contain the k smallest elements in sorted order.
/// This invariant must hold at the start and end of each iteration.</description></item>
/// <item><description><strong>Minimum Selection:</strong> For each position i in [0..n-1), correctly identify the minimum element
/// in the unsorted region [i..n). This requires comparing the candidate minimum with every element in the unsorted portion,
/// ensuring no smaller element is overlooked.</description></item>
/// <item><description><strong>Swap Operation:</strong> Exchange the minimum element from the unsorted region with the element at position i.
/// This places the (i+1)-th smallest element at index i, extending the sorted region by one.
/// Skip the swap if the minimum is already at position i (optimization that doesn't affect correctness).</description></item>
/// <item><description><strong>Boundary Advancement:</strong> After each swap, increment the boundary index i by 1.
/// This shrinks the unsorted region and grows the sorted region until the entire array is sorted.
/// Terminate when i reaches n-1 (only one element remains, which is automatically in place).</description></item>
/// <item><description><strong>Comparison Consistency:</strong> All element comparisons must use a total order relation (transitive, antisymmetric, total).
/// The IComparable&lt;T&gt;.CompareTo implementation must satisfy these properties for correctness.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Selection</description></item>
/// <item><description>Stable      : No (swapping non-adjacent elements can change relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space)</description></item>
/// <item><description>Best case   : Θ(n²) - Always performs n(n-1)/2 comparisons regardless of input order</description></item>
/// <item><description>Average case: Θ(n²) - Same comparison count; swap count varies but doesn't dominate</description></item>
/// <item><description>Worst case  : Θ(n²) - Same comparison count; maximum n-1 swaps when reverse sorted</description></item>
/// <item><description>Comparisons : Θ(n²) - Exactly n(n-1)/2 comparisons in all cases (input-independent)</description></item>
/// <item><description>Swaps       : O(n) - At most n-1 swaps; best case 0 (already sorted), worst case n-1</description></item>
/// <item><description>Writes      : O(n) - 2 writes per swap (via tuple deconstruction or temp variable)</description></item>
/// </list>
/// <para><strong>Use Cases:</strong></para>
/// <list type="bullet">
/// <item><description>Small datasets where simplicity is valued over performance</description></item>
/// <item><description>Situations where write operations are expensive (minimizes swaps compared to bubble sort)</description></item>
/// <item><description>Educational purposes to teach fundamental sorting concepts</description></item>
/// <item><description>When memory writes are costly but comparisons are cheap</description></item>
/// </list>
/// </remarks>
/// <typeparam name="T"></typeparam>
public static class SelectionSort
{
    /// <summary>
    /// Sorts the elements in the specified span in ascending order using the default comparer.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort in place.</param>
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
    {
        Sort(span, 0, span.Length, NullContext.Default);
    }

    /// <summary>
    /// Sorts the elements in the specified span using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        Sort(span, 0, span.Length, context);
    }

    /// <summary>
    /// Sorts the elements in the specified range of a span using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span containing the elements to sort.</param>
    /// <param name="first">The zero-based index of the first element in the range to sort. Must be greater than or equal to 0 and less than
    /// <paramref name="last"/>.</param>
    /// <param name="last">The exclusive upper bound of the range to sort. Must be less than or equal to the length of <paramref
    /// name="span"/>.</param>
    /// <param name="context">The sort context to use during the sorting operation.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="first"/> is less than 0, <paramref name="last"/> is greater than the length of
    /// <paramref name="span"/>, or <paramref name="first"/> is greater than or equal to <paramref name="last"/>.</exception>
    internal static void Sort<T>(Span<T> span, int first, int last, ISortContext context) where T : IComparable<T>
    {
        Debug.Assert(first >= 0 && last <= span.Length && first < last, "Invalid range for sorting.");

        if (span.Length <= 1) return;

        var s = new SortSpan<T>(span, context);

        for (var i = first; i < last - 1; i++)
        {
            var min = i;

            // Find the index of the minimum element
            for (var j = i + 1; j < last; j++)
            {
                if (s.Compare(j, min) < 0)
                {
                    min = j;
                }
            }

            // Swap the found minimum element with the first element of the unsorted part
            if (min != i)
            {
                s.Swap(min, i);
            }
        }
    }
}
