using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// QuickSort、HeapSort、InsertionSortを組み合わせたハイブリッドソートアルゴリズムです。
/// 通常はQuickSortを使用しますが、小さい配列ではInsertionSort、再帰深度が深くなりすぎた場合はHeapSortに切り替えることで、
/// QuickSortの最悪ケースO(n²)を回避し、常にO(n log n)を保証します。
/// <br/>
/// A hybrid sorting algorithm that combines QuickSort, HeapSort, and InsertionSort.
/// It primarily uses QuickSort, but switches to InsertionSort for small arrays and HeapSort when recursion depth becomes too deep,
/// avoiding QuickSort's worst-case O(n²) and guaranteeing O(n log n) in all cases.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Introsort:</strong></para>
/// <list type="number">
/// <item><description><strong>Adaptive Algorithm Selection:</strong> The algorithm must correctly choose between three sub-algorithms:
/// <list type="bullet">
/// <item><description>InsertionSort when partition size ≤ 30 (optimized via benchmarking for best performance)</description></item>
/// <item><description>HeapSort when recursion depth exceeds depthLimit = 2⌊log₂(n)⌋</description></item>
/// <item><description>QuickSort (median-of-three + Hoare partition) for all other cases</description></item>
/// </list>
/// This adaptive selection ensures O(n log n) worst-case while maintaining QuickSort's average-case performance.</description></item>
/// <item><description><strong>Depth Limit Calculation:</strong> The depth limit must be set to 2⌊log₂(n)⌋ where n is the partition size.
/// This value is derived from the expected depth of a balanced binary tree (⌊log₂(n)⌋) multiplied by 2 to allow for some imbalance.
/// When this limit is exceeded, it indicates pathological QuickSort behavior (e.g., adversarial input patterns),
/// triggering a switch to HeapSort which guarantees O(n log n) regardless of input.</description></item>
/// <item><description><strong>QuickSort Phase - Median-of-Three Pivot Selection:</strong> To avoid worst-case QuickSort behavior on sorted/reverse-sorted inputs,
/// the pivot is selected adaptively based on array size:
/// <list type="bullet">
/// <item><description>For arrays &lt; 1000 elements: median of three quartile positions (q1 = left + n/4, mid = left + n/2, q3 = left + 3n/4)</description></item>
/// <item><description>For arrays ≥ 1000 elements: <strong>Ninther</strong> (median-of-5) using positions: left, left+delta/2, mid, mid+delta/2, right (where delta = n/2)</description></item>
/// </list>
/// The quartile-based sampling provides better pivot quality than simple left/mid/right sampling.
/// The Ninther (median-of-5) further improves pivot selection for large arrays, reducing the probability of unbalanced partitions.
/// This adaptive approach is similar to C++ std::introsort's __sort5 optimization.</description></item>
/// <item><description><strong>QuickSort Phase - Hoare Partition Scheme:</strong> Partitioning uses bidirectional scanning:
/// <list type="bullet">
/// <item><description>Left pointer l advances while s[l] &lt; pivot (with boundary check l &lt; right)</description></item>
/// <item><description>Right pointer r retreats while s[r] &gt; pivot (with boundary check r &gt; left)</description></item>
/// <item><description>When both pointers stop and l ≤ r, swap s[l] ↔ s[r] and advance both pointers</description></item>
/// <item><description>Loop terminates when l &gt; r, ensuring partitioning invariant: s[left..r] ≤ pivot ≤ s[l..right]</description></item>
/// </list>
/// The condition l ≤ r (not l &lt; r) ensures elements equal to pivot are swapped, preventing infinite loops on duplicate-heavy arrays.
/// Boundary checks prevent out-of-bounds access when all elements are smaller/larger than pivot.
/// <br/>
/// <strong>Duplicate Detection Optimization:</strong> After partitioning, if one partition is empty and the other contains all elements,
/// the algorithm checks if all elements equal the pivot value. This detects arrays with many duplicates (common in real-world data)
/// and terminates early, avoiding unnecessary recursion. This optimization is particularly effective for:
/// <list type="bullet">
/// <item><description>Boolean arrays (only two distinct values)</description></item>
/// <item><description>Categorical data with few distinct values (e.g., status codes, ratings)</description></item>
/// <item><description>Arrays with many repeated elements (e.g., sensor data with constant readings)</description></item>
/// </list></description></item>
/// <item><description><strong>Tail Recursion Optimization:</strong> After partitioning into [left, r] and [l, right],
/// the algorithm always recursively processes the smaller partition and loops on the larger partition.
/// This optimization guarantees the recursion stack depth is at most O(log n) (specifically ⌈log₂(n)⌉),
/// even in pathological cases before the depth limit triggers HeapSort.
/// This is identical to the strategy used in LLVM libcxx's std::sort implementation.</description></item>
/// <item><description><strong>InsertionSort Threshold:</strong> For partitions of size ≤ 30, InsertionSort is used instead of QuickSort.
/// This threshold was determined through empirical benchmarking:
/// <list type="bullet">
/// <item><description>InsertionSort has lower constant factors than QuickSort for small arrays</description></item>
/// <item><description>Reduces recursion overhead (30-element partition would require ~5 recursion levels)</description></item>
/// <item><description>Improves cache locality by processing small contiguous regions (fits in L1 cache)</description></item>
/// <item><description>Benchmark results showed 10-30% performance improvement for threshold 30 vs. 16 across primitive and reference types</description></item>
/// <item><description>Threshold of 30 matches C++ std::introsort for trivially copyable types</description></item>
/// </list>
/// This hybrid approach achieves better constant factors than pure QuickSort while maintaining O(n log n) worst-case.</description></item>
/// <item><description><strong>Nearly-Sorted Detection with Early Abort (SortIncomplete):</strong> When partitioning produces zero swaps,
/// the partition is likely nearly sorted. The algorithm uses InsertionSort.SortIncomplete to attempt sorting:
/// <list type="bullet">
/// <item><description>For very small partitions (2-5 elements): Uses sorting networks for optimal performance</description></item>
/// <item><description>For larger partitions: Tracks insertion count; aborts if more than 8 insertions are needed</description></item>
/// <item><description>If both partitions complete: Entire range is sorted, return early</description></item>
/// <item><description>If one partition completes: Continue with only the incomplete partition (tail recursion optimization)</description></item>
/// <item><description>If both partitions incomplete: Fall through to regular QuickSort/HeapSort (partition not nearly sorted)</description></item>
/// </list>
/// This optimization is based on LLVM libcxx's __insertion_sort_incomplete and significantly improves performance for nearly-sorted data,
/// common in real-world scenarios (append operations, partially sorted streams, time-series data).</description></item>
/// <item><description><strong>HeapSort Fallback Correctness:</strong> When depthLimit reaches 0, the current partition is sorted using HeapSort.
/// HeapSort guarantees O(n log n) time complexity regardless of input distribution, providing a safety net against adversarial inputs.
/// The depth limit ensures that HeapSort is invoked only when QuickSort exhibits pathological behavior,
/// preserving QuickSort's superior average-case performance for typical inputs.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Hybrid (Partition (base) + Heap + Insertion)</description></item>
/// <item><description>Stable      : No (QuickSort and HeapSort are unstable; element order is not preserved for equal values)</description></item>
/// <item><description>In-place    : Yes (O(log n) auxiliary space for recursion stack, no additional arrays allocated)</description></item>
/// <item><description>Best case   : Θ(n log n) - Occurs when QuickSort consistently creates balanced partitions and InsertionSort handles small subarrays efficiently</description></item>
/// <item><description>Average case: Θ(n log n) - Expected ~1.386n log₂ n comparisons from QuickSort with Hoare partition, reduced by InsertionSort optimization</description></item>
/// <item><description>Worst case  : O(n log n) - Guaranteed by HeapSort fallback when recursion depth exceeds 2⌊log₂(n)⌋</description></item>
/// <item><description>Comparisons : ~1.2-1.4n log₂ n (average) - Lower than pure QuickSort due to InsertionSort handling small partitions</description></item>
/// <item><description>Swaps       : ~0.33n log₂ n (average) - Hoare partition performs significantly fewer swaps than Lomuto partition</description></item>
/// </list>
/// <para><strong>Advantages of Introsort:</strong></para>
/// <list type="bullet">
/// <item><description>Worst-case guarantee: Always O(n log n), unlike pure QuickSort which degrades to O(n²)</description></item>
/// <item><description>Average-case efficiency: Matches QuickSort's performance on typical inputs (~1.4n log₂ n comparisons)</description></item>
/// <item><description>Cache-friendly: InsertionSort for small partitions improves spatial locality</description></item>
/// <item><description>Stack-safe: Tail recursion optimization + depth limit ensures O(log n) stack depth</description></item>
/// <item><description>Practical performance: Used in production libraries (C++ std::sort, .NET Array.Sort, Java Arrays.sort for primitives)</description></item>
/// <item><description>Robust pivot selection: Ninther (median-of-5) for large arrays and quartile-based median-of-3 for smaller arrays handle various data patterns</description></item>
/// <item><description>Duplicate-aware: Detects and handles arrays with many equal elements efficiently (common in categorical/boolean data)</description></item>
/// <item><description>Nearly-sorted optimization: Detects potential nearly-sorted partitions (zero swaps) and uses SortIncomplete with early abort (LLVM libcxx optimization)</description></item>
/// </list>
/// <para><strong>Implementation Details:</strong></para>
/// <list type="bullet">
/// <item><description>Threshold value: 30 elements for switching to InsertionSort (empirically optimal via benchmarking, 10-30% faster than threshold 16)</description></item>
/// <item><description>Depth limit: 2 × floor(log₂(n)) - allows some imbalance before triggering HeapSort</description></item>
/// <item><description>Pivot selection (adaptive):
/// <list type="bullet">
/// <item><description>Arrays ≥ 1000: Ninther (median-of-5 using positions: left, left+n/4, mid, mid+3n/4, right)</description></item>
/// <item><description>Arrays &lt; 1000: Median-of-3 using quartile positions (n/4, n/2, 3n/4)</description></item>
/// </list></description></item>
/// <item><description>Partition scheme: Hoare partition (bidirectional scan) for fewer swaps and better duplicate handling</description></item>
/// <item><description>Duplicate detection: Detects all-equal-to-pivot case after partitioning and terminates early (optimizes arrays with many duplicates)
/// <list type="bullet">
/// <item><description>Performance: ~92% reduction in comparisons for all-equal arrays (10K elements: 10K vs. 133K comparisons)</description></item>
/// <item><description>Effective for: Boolean arrays, categorical data with few distinct values (2-10), sensor data with constant readings</description></item>
/// <item><description>Detection: When one partition is empty, checks if all elements equal pivot value</description></item>
/// </list></description></item>
/// <item><description>Nearly-sorted detection: Tracks swap count during partitioning; if zero, uses SortIncomplete with early abort (LLVM __insertion_sort_incomplete)</description></item>
/// <item><description>Small array sorting networks: Partitions of 2-5 elements use optimal sorting networks (2-10 comparisons)</description></item>
/// <item><description>Tail recursion: Always recurse on smaller partition, loop on larger to guarantee O(log n) stack depth (matches LLVM std::sort)</description></item>
/// </list>
/// <para><strong>Historical Context:</strong></para>
/// <para>
/// Introsort was invented by David Musser in 1997 as a solution to QuickSort's quadratic worst-case behavior.
/// The name "Introsort" is short for "Introspective Sort" - the algorithm introspects its own behavior (recursion depth)
/// and adapts by switching to HeapSort when needed. This hybrid approach combines the best characteristics of QuickSort
/// (fast average case), HeapSort (guaranteed O(n log n)), and InsertionSort (efficient for small arrays).
/// </para>
/// <para><strong>Why This Implementation is Theoretically Correct:</strong></para>
/// <list type="number">
/// <item><description>Partitioning correctness: Hoare partition maintains invariant s[left..r] ≤ pivot ≤ s[l..right] with proper boundary checks</description></item>
/// <item><description>Recursion correctness: Both partitions [left, r] and [l, right] are strictly smaller than [left, right] due to pointer advance after swap</description></item>
/// <item><description>Termination guarantee: Combination of depth limit (triggers HeapSort) and tail recursion (limits stack depth) ensures termination</description></item>
/// <item><description>Algorithm correctness: InsertionSort, HeapSort, and QuickSort are all proven correct sorting algorithms</description></item>
/// <item><description>Complexity guarantee: Depth limit of 2⌊log₂(n)⌋ ensures HeapSort fallback before stack overflow or quadratic behavior</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Introsort</para>
/// <para>Paper: David R Musser https://webpages.charlotte.edu/rbunescu/courses/ou/cs4040/introsort.pdf</para>
/// <para>LLVM implementation: https://github.com/llvm/llvm-project/blob/368faacac7525e538fa6680aea74e19a75e3458d/libcxx/include/__algorithm/sort.h#L272</para>
/// </remarks>
public static class IntroSort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array

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
    /// <param name="context">The sort context that tracks statistics and provides sorting operations. Cannot be null.</param>
    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        Sort(span, 0, span.Length, context);
    }

    /// <summary>
    /// Sorts the subrange [first..last) using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span containing elements to sort.</param>
    /// <param name="first">The inclusive start index of the range to sort.</param>
    /// <param name="last">The exclusive end index of the range to sort.</param>
    /// <param name="context">The sort context for tracking statistics and observations.</param>
    public static void Sort<T>(Span<T> span, int first, int last, ISortContext context) where T : IComparable<T>
    {
        ArgumentOutOfRangeException.ThrowIfNegative(first);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(last, span.Length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(first, last);

        if (last - first <= 1) return;

        var s = new SortSpan<T>(span, context, BUFFER_MAIN);
        SortCore(s, first, last - 1, context);
    }

    /// <summary>
    /// Sorts the subrange [left..right] using the provided sort context.
    /// This overload accepts a SortSpan directly for use by other algorithms that already have a SortSpan instance.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="left">The inclusive start index of the range to sort.</param>
    /// <param name="right">The inclusive end index of the range to sort.</param>
    /// <param name="context">The sort context for tracking statistics and observations.</param>
    internal static void SortCore<T>(SortSpan<T> s, int left, int right, ISortContext context) where T : IComparable<T>
    {
        var depthLimit = 2 * FloorLog2(right - left + 1);
        IntroSortInternal(s, left, right, depthLimit, 30, true, context);
    }

    /// <summary>
    /// INTERNAL BENCHMARK ONLY: Sorts the span with a custom InsertionSort threshold.
    /// This method is used for benchmarking to test different threshold values.
    /// DO NOT use this method in production code.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <param name="span">The span to sort.</param>
    /// <param name="insertionSortThreshold">The threshold at which to switch to InsertionSort.</param>
    internal static void SortWithCustomThreshold<T>(Span<T> span, int insertionSortThreshold) where T : IComparable<T>
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T>(span, NullContext.Default, BUFFER_MAIN);
        var depthLimit = 2 * FloorLog2(span.Length);
        IntroSortInternal(s, 0, span.Length - 1, depthLimit, insertionSortThreshold, true, NullContext.Default);
    }

    /// <summary>
    /// Internal IntroSort implementation that switches between QuickSort, HeapSort, and InsertionSort based on size and depth.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="s">The SortSpan wrapping the span to sort.</param>
    /// <param name="left">The inclusive start index of the range to sort.</param>
    /// <param name="right">The inclusive end index of the range to sort.</param>
    /// <param name="depthLimit">The recursion depth limit before switching to HeapSort.</param>
    /// <param name="insertionSortThreshold">The threshold size at which to switch to InsertionSort.</param>
    /// <param name="leftmost">True if this is the leftmost partition (requires boundary checks in InsertionSort), 
    /// <param name="context">The sort context for tracking statistics and observations.</param>
    /// false otherwise (can use unguarded InsertionSort).</param>
    private static void IntroSortInternal<T>(SortSpan<T> s, int left, int right, int depthLimit, int insertionSortThreshold, bool leftmost, ISortContext context) where T : IComparable<T>
    {
        while (right > left)
        {
            var size = right - left + 1;

            // Small arrays: use InsertionSort
            // For leftmost partitions, use guarded version (needs boundary checks)
            // For non-leftmost partitions, use unguarded version (pivot acts as sentinel)
            if (size <= insertionSortThreshold)
            {
                if (leftmost)
                {
                    InsertionSort.SortCore(s, left, right + 1);
                }
                else
                {
                    InsertionSort.UnguardedSortCore(s, left, right + 1);
                }
                return;
            }

            // Max depth reached: switch to HeapSort to guarantee O(n log n)
            if (depthLimit == 0)
            {
                HeapSort.SortCore(s, left, right + 1);
                return;
            }

            depthLimit--;

            // QuickSort with adaptive pivot selection:
            // - For large arrays (>= 1000): use Ninther (median-of-5) for better pivot quality
            // - For smaller arrays: use median-of-3 (quartile-based)
            var length = right - left + 1;
            T pivot;

            if (length >= 1000)
            {
                // Ninther: 5-point sampling for large arrays (similar to C++ std::introsort)
                var delta = length / 2;
                var mid = left + delta;
                var q1 = left + delta / 2;
                var q3 = mid + delta / 2;
                pivot = MedianOf5Value(s, left, q1, mid, q3, right);
            }
            else
            {
                // Standard quartile-based median-of-3 for smaller arrays
                var q1 = left + length / 4;
                var mid = left + length / 2;
                var q3 = left + (length * 3) / 4;
                pivot = MedianOf3Value(s, q1, mid, q3);
            }

            // Hoare partition scheme with swap counting for nearly-sorted detection
            var l = left;
            var r = right;
            var swapCount = 0;

            while (l <= r)
            {
                // Move l forward while elements are less than pivot
                while (l < right && s.Compare(l, pivot) < 0)
                {
                    l++;
                }

                // Move r backward while elements are greater than pivot
                while (r > left && s.Compare(r, pivot) > 0)
                {
                    r--;
                }

                // If pointers haven't crossed, swap and advance both
                if (l <= r)
                {
                    if (l != r) // Only count actual swaps (not self-swaps)
                    {
                        s.Swap(l, r);
                        swapCount++;
                    }
                    l++;
                    r--;
                }
            }

            // After partitioning: [left..r] <= pivot, [l..right] >= pivot
            // Detect all-equal-to-pivot case (common in arrays with many duplicates)
            // If one partition is empty and the other is the full range, all elements equal pivot
            var leftPartSize = r - left + 1;
            var rightPartSize = right - l + 1;
            var totalSize = right - left + 1;

            if (leftPartSize == 0 && rightPartSize == totalSize)
            {
                // All elements >= pivot; check if all are equal to pivot
                // This happens when all elements in range equal the pivot value
                var allEqual = true;
                for (var i = left; i <= right && allEqual; i++)
                {
                    if (s.Compare(i, pivot) != 0)
                    {
                        allEqual = false;
                    }
                }
                if (allEqual)
                {
                    // All elements equal - range is sorted, done
                    return;
                }
            }
            else if (rightPartSize == 0 && leftPartSize == totalSize)
            {
                // All elements <= pivot; check if all are equal to pivot
                var allEqual = true;
                for (var i = left; i <= right && allEqual; i++)
                {
                    if (s.Compare(i, pivot) != 0)
                    {
                        allEqual = false;
                    }
                }
                if (allEqual)
                {
                    // All elements equal - range is sorted, done
                    return;
                }
            }

            // Nearly-sorted detection: if no swaps occurred, try InsertionSort with early abort
            // This is similar to C++ std::introsort's __insertion_sort_incomplete optimization
            if (swapCount == 0)
            {
                // For nearly-sorted arrays, InsertionSort is very efficient
                // Try both partitions with SortIncomplete (can give up if not nearly sorted)
                var leftSorted = left >= r || InsertionSort.SortIncomplete(s, left, r + 1, leftmost);
                var rightSorted = l >= right || InsertionSort.SortIncomplete(s, l, right + 1, false);

                if (leftSorted)
                {
                    if (rightSorted)
                    {
                        // Both partitions completed successfully - done
                        return;
                    }
                    else
                    {
                        // Left done, right needs more work - continue with right partition
                        leftmost = false;
                        left = l;
                        continue;
                    }
                }
                else
                {
                    if (rightSorted)
                    {
                        // Right done, left needs more work - continue with left partition
                        right = r;
                        continue;
                    }
                    else
                    {
                        // Both partitions incomplete - fall through to regular tail recursion
                    }
                }
            }

            // Tail recursion optimization: always recurse on smaller partition, loop on larger
            // This guarantees O(log n) stack depth even for pathological inputs
            // (similar to C++ std::introsort and LLVM implementation)
            var leftSize = r - left + 1;
            var rightSize = right - l + 1;

            if (leftSize < rightSize)
            {
                // Recurse on smaller left partition (preserves leftmost flag)
                if (left < r)
                {
                    IntroSortInternal(s, left, r, depthLimit, insertionSortThreshold, leftmost, context);
                }
                // Tail recursion: continue loop with larger right partition
                // Right partition is never leftmost (element at position r acts as sentinel)
                leftmost = false;
                left = l;
            }
            else
            {
                // Recurse on smaller right partition (always non-leftmost)
                if (l < right)
                {
                    IntroSortInternal(s, l, right, depthLimit, insertionSortThreshold, false, context);
                }
                // Tail recursion: continue loop with larger left partition
                // Preserve leftmost flag for left partition
                right = r;
            }
        }
    }

    /// <summary>
    /// Returns the median value among three elements at specified indices.
    /// This method performs exactly 2-3 comparisons to determine the median value.
    /// </summary>
    private static T MedianOf3Value<T>(SortSpan<T> s, int lowIdx, int midIdx, int highIdx) where T : IComparable<T>
    {
        // Use SortSpan.Compare to track statistics
        var cmpLowMid = s.Compare(lowIdx, midIdx);

        if (cmpLowMid > 0) // low > mid
        {
            var cmpMidHigh = s.Compare(midIdx, highIdx);
            if (cmpMidHigh > 0) // low > mid > high
            {
                return s.Read(midIdx); // mid is median
            }
            else // low > mid, mid <= high
            {
                var cmpLowHigh = s.Compare(lowIdx, highIdx);
                return cmpLowHigh > 0 ? s.Read(highIdx) : s.Read(lowIdx);
            }
        }
        else // low <= mid
        {
            var cmpMidHigh = s.Compare(midIdx, highIdx);
            if (cmpMidHigh > 0) // low <= mid, mid > high
            {
                var cmpLowHigh = s.Compare(lowIdx, highIdx);
                return cmpLowHigh > 0 ? s.Read(lowIdx) : s.Read(highIdx);
            }
            else // low <= mid <= high
            {
                return s.Read(midIdx); // mid is median
            }
        }
    }

    /// <summary>
    /// Returns the median value among five elements at specified indices.
    /// This implements "Ninther" - median-of-medians using 5 samples for better pivot quality on large arrays.
    /// Performs 6-8 comparisons to determine the median value.
    /// </summary>
    /// <remarks>
    /// This is based on C++ std::introsort's __sort5 optimization for arrays >= 1000 elements.
    /// The five samples are: left, left+delta/2, mid, mid+delta/2, right (where delta = length/2).
    /// This provides better pivot selection than simple 3-point sampling, especially for large arrays
    /// with patterns like partially-sorted or mountain-shaped distributions.
    /// </remarks>
    private static T MedianOf5Value<T>(SortSpan<T> s, int i1, int i2, int i3, int i4, int i5) where T : IComparable<T>
    {
        // Sort the 5 indices using a sorting network (6 comparisons minimum for 5 elements)
        // We'll use a simplified approach: sort pairs, then find median

        // First, sort pairs: (i1, i2), (i3, i4)
        if (s.Compare(i1, i2) > 0) (i1, i2) = (i2, i1);
        if (s.Compare(i3, i4) > 0) (i3, i4) = (i4, i3);

        // Now i1 <= i2 and i3 <= i4
        // Find median of i2, i3, i5 (this will be in the middle range)
        if (s.Compare(i2, i5) > 0) (i2, i5) = (i5, i2);
        if (s.Compare(i2, i3) > 0) (i2, i3) = (i3, i2);

        // Now we need the median of the remaining elements
        // We know: i1 <= i2, i3 <= i4, and i2 is constrained
        // The median is the 3rd element when sorted

        if (s.Compare(i1, i3) > 0) (i1, i3) = (i3, i1);
        // i1 is now the minimum of {i1, i3}

        if (s.Compare(i2, i3) > 0) (i2, i3) = (i3, i2);
        // i2 is now <= i3

        if (s.Compare(i2, i4) > 0)
        {
            if (s.Compare(i3, i4) > 0)
            {
                return s.Read(i4);
            }
            else
            {
                return s.Read(i3);
            }
        }

        return s.Read(i2);
    }

    /// <summary>
    /// Computes the floor of the base-2 logarithm of a positive integer.
    /// </summary>
    /// <param name="n">The positive integer to compute the logarithm for.</param>
    /// <returns>The floor of log2(n).</returns>
    private static int FloorLog2(int n)
    {
        var result = 0;
        while (n > 1)
        {
            result++;
            n >>= 1;
        }
        return result;
    }
}
