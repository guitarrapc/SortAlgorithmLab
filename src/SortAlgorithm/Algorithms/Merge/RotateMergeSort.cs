using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 配列を再帰的に半分に分割し、それぞれをソートした後、回転アルゴリズムを使用してインプレースでマージする分割統治アルゴリズムです。
/// 安定ソートであり、追加メモリを使用せずにO(n log n)の性能を保証します。
/// 回転をするため、要素の移動が多くなるため、標準のマージソートよりも遅くなります。
/// <br/>
/// Recursively divides the array in half, sorts each part, then merges sorted subarrays in-place using array rotation.
/// This divide-and-conquer algorithm is stable and guarantees O(n log n) performance without requiring auxiliary space.
/// However, due to the rotations, it involves more element movements and is slower than standard merge sort.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Rotate Merge Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Divide Step (Binary Partitioning):</strong> The array must be divided into two roughly equal halves at each recursion level.
/// The midpoint is calculated as mid = (left + right) / 2, ensuring balanced subdivision.
/// This guarantees a recursion depth of ⌈log₂(n)⌉.</description></item>
/// <item><description><strong>Base Case (Termination Condition):</strong> Recursion must terminate when a subarray has size ≤ 1.
/// An array of size 0 or 1 is trivially sorted and requires no further processing.</description></item>
/// <item><description><strong>Conquer Step (Recursive Sorting):</strong> Each half must be sorted independently via recursive calls.
/// The left subarray [left..mid] and right subarray [mid+1..right] are sorted before merging.</description></item>
/// <item><description><strong>In-Place Merge Step:</strong> Two sorted subarrays must be merged without using additional memory.
/// This is achieved using array rotation, which rearranges elements by shifting blocks of the array.</description></item>
/// <item><description><strong>Rotation Algorithm (Block Reversal):</strong> Array rotation is implemented using three reversals:
/// To rotate array A of length n by k positions: Reverse(A[0..k-1]), Reverse(A[k..n-1]), Reverse(A[0..n-1]).
/// This achieves O(n) time rotation with O(1) space and preserves element stability.</description></item>
/// <item><description><strong>Merge via Rotation:</strong> During merge, find the position where the first element of the right partition
/// should be inserted in the left partition (using binary search). Rotate elements to place it correctly, then recursively
/// merge the remaining elements. This maintains sorted order while being in-place.</description></item>
/// <item><description><strong>Stability Preservation:</strong> Binary search uses &lt;= comparison to find the insertion position,
/// ensuring equal elements from the left partition appear before equal elements from the right partition.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Merge (In-Place variant)</description></item>
/// <item><description>Stable      : Yes (binary search with &lt;= comparison preserves relative order)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space, uses rotation instead of buffer)</description></item>
/// <item><description>Best case   : O(n log n) - Even sorted data requires ⌈log₂(n)⌉ levels of merging</description></item>
/// <item><description>Average case: O(n log² n) - Binary search (log n) + rotation (n) per merge level (log n levels)</description></item>
/// <item><description>Worst case  : O(n log² n) - Rotation adds O(n) factor to each merge operation</description></item>
/// <item><description>Comparisons : O(n log² n) - Binary search adds log n comparisons per merge</description></item>
/// <item><description>Writes      : O(n² log n) - Rotation requires multiple element movements (n writes per level)</description></item>
/// <item><description>Space       : O(log n) - Only recursion stack space, no auxiliary buffer needed</description></item>
/// </list>
/// <para><strong>Advantages of Rotate Merge Sort:</strong></para>
/// <list type="bullet">
/// <item><description>True in-place sorting - O(1) auxiliary space (only recursion stack)</description></item>
/// <item><description>Stable - Preserves relative order of equal elements</description></item>
/// <item><description>Predictable performance - O(n log² n) guaranteed in all cases</description></item>
/// <item><description>Cache-friendly - Better locality than standard merge sort with buffer</description></item>
/// </list>
/// <para><strong>Disadvantages:</strong></para>
/// <list type="bullet">
/// <item><description>Slower than buffer-based merge sort - Additional log n factor from binary search and rotation overhead</description></item>
/// <item><description>More writes than standard merge sort - Rotation requires multiple element movements</description></item>
/// <item><description>Not adaptive - Doesn't exploit existing order in data</description></item>
/// </list>
/// <para><strong>Use Cases:</strong></para>
/// <list type="bullet">
/// <item><description>When memory is extremely constrained (embedded systems, real-time systems)</description></item>
/// <item><description>When stability is required but auxiliary memory is not available</description></item>
/// <item><description>Educational purposes - Understanding in-place merging techniques</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Merge_sort#Variants</para>
/// <para>Rotation-based in-place merge: Practical In-Place Merging (Geffert et al.)</para>
/// </remarks>
public static class RotateMergeSort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array (in-place operations only)

    /// <summary>
    /// Sorts the elements in the specified span in ascending order using the default comparer.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort in place.</param>
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
    {
        Sort(span, NullContext.Default);
    }

    /// <summary>
    /// Sorts the elements in the specified span using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T>(span, context, BUFFER_MAIN);
        SortCore(s, 0, span.Length - 1);
    }

    /// <summary>
    /// Core recursive merge sort implementation.
    /// </summary>
    /// <param name="s">The SortSpan wrapping the span to sort</param>
    /// <param name="left">The inclusive start index of the range to sort</param>
    /// <param name="right">The inclusive end index of the range to sort</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SortCore<T>(SortSpan<T> s, int left, int right) where T : IComparable<T>
    {
        if (right <= left) return; // Base case: array of size 0 or 1 is sorted

        var mid = left + (right - left) / 2;

        // Conquer: Recursively sort left and right halves
        SortCore(s, left, mid);
        SortCore(s, mid + 1, right);

        // Optimization: Skip merge if already sorted (left[last] <= right[first])
        if (s.Compare(mid, mid + 1) <= 0)
        {
            return; // Already sorted, no merge needed
        }

        // Merge: Combine two sorted halves in-place using rotation
        MergeInPlace(s, left, mid, right);
    }

    /// <summary>
    /// Merges two sorted subarrays [left..mid] and [mid+1..right] in-place using rotation.
    /// Uses binary search to find insertion points and rotation to rearrange elements.
    /// Optimization: Processes multiple consecutive elements from the right partition at once.
    /// </summary>
    /// <param name="s">The SortSpan wrapping the array</param>
    /// <param name="left">The inclusive start index of the left subarray</param>
    /// <param name="mid">The inclusive end index of the left subarray</param>
    /// <param name="right">The inclusive end index of the right subarray</param>
    private static void MergeInPlace<T>(SortSpan<T> s, int left, int mid, int right) where T : IComparable<T>
    {
        var start1 = left;
        var start2 = mid + 1;

        // Main merge loop using rotation algorithm
        while (start1 <= mid && start2 <= right)
        {
            // If element at start1 is in correct position
            if (s.Compare(start1, start2) <= 0)
            {
                start1++;
            }
            else
            {
                // Optimization: Find how many consecutive elements from right partition
                // can be moved to the current position in left partition
                var value = s.Read(start2);
                var insertPos = BinarySearch(s, start1, mid, value);
                
                // Find the end of consecutive elements in right partition that belong here
                var start2End = start2;
                while (start2End < right && s.Compare(insertPos, start2End + 1) > 0)
                {
                    start2End++;
                }
                
                var blockSize = start2End - start2 + 1;
                var rotateDistance = start2 - insertPos;
                
                // Rotate the block [insertPos..start2End] to move all elements at once
                Rotate(s, insertPos, start2End, rotateDistance);

                // Update pointers after moving the block
                start1 = insertPos + blockSize;
                mid += blockSize;
                start2 = start2End + 1;
            }
        }
    }

    /// <summary>
    /// Rotates a subarray by k positions to the left using the reversal algorithm.
    /// Rotation is achieved by three reversals: Reverse[0..k-1], Reverse[k..n-1], Reverse[0..n-1].
    /// </summary>
    /// <param name="s">The SortSpan wrapping the array</param>
    /// <param name="left">The start index of the subarray to rotate</param>
    /// <param name="right">The end index of the subarray to rotate</param>
    /// <param name="k">The number of positions to rotate left</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Rotate<T>(SortSpan<T> s, int left, int right, int k) where T : IComparable<T>
    {
        if (k == 0 || left >= right) return;

        // Normalize k to be within range
        var n = right - left + 1;
        k = k % n;
        if (k == 0) return;

        // Three-reversal rotation algorithm
        Reverse(s, left, left + k - 1);
        Reverse(s, left + k, right);
        Reverse(s, left, right);
    }

    /// <summary>
    /// Reverses a subarray in-place.
    /// </summary>
    /// <param name="s">The SortSpan wrapping the array</param>
    /// <param name="left">The start index of the subarray to reverse</param>
    /// <param name="right">The end index of the subarray to reverse</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Reverse<T>(SortSpan<T> s, int left, int right) where T : IComparable<T>
    {
        while (left < right)
        {
            s.Swap(left, right);
            left++;
            right--;
        }
    }

    /// <summary>
    /// Performs binary search to find the insertion position for a value in a sorted range.
    /// Uses &lt;= comparison to maintain stability (insert after equal elements).
    /// </summary>
    /// <param name="s">The SortSpan wrapping the array</param>
    /// <param name="left">The start index of the search range</param>
    /// <param name="right">The end index of the search range</param>
    /// <param name="value">The value to search for</param>
    /// <returns>The index where the value should be inserted</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int BinarySearch<T>(SortSpan<T> s, int left, int right, T value) where T : IComparable<T>
    {
        while (left <= right)
        {
            var mid = left + (right - left) / 2;
            var cmp = s.Compare(s.Read(mid), value);

            if (cmp <= 0)
            {
                left = mid + 1;
            }
            else
            {
                right = mid - 1;
            }
        }

        return left;
    }
}
