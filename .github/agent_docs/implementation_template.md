# Sorting Algorithm Implementation Template

Use this template as a starting point for implementing new sorting algorithms.

```csharp
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Sortings;

/// <summary>
/// {Algorithm Name} sorting algorithm.
/// Time Complexity: O(?) average, O(?) worst
/// Space Complexity: O(?)
/// Stable: Yes/No
/// </summary>
public static class MySort
{
    private const int InsertionSortThreshold = 16;

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Temporary buffer (if needed)

    /// <summary>
    /// Sorts the span using {Algorithm Name}.
    /// </summary>
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
        => Sort(span, NullContext.Default);

    /// <summary>
    /// Sorts the span using {Algorithm Name} with context tracking.
    /// </summary>
    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T>(span, context, BUFFER_MAIN);

        // Use insertion sort for small arrays
        if (s.Length <= InsertionSortThreshold)
        {
            InsertionSort.Sort(span, context);
            return;
        }

        SortCore(s, 0, s.Length - 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SortCore<T>(SortSpan<T> s, int left, int right) where T : IComparable<T>
    {
        // Implementation here
        // Use s.Read(), s.Write(), s.Compare(), s.Swap() consistently
    }

    // Additional helper methods as needed
}
```

## Key Points

1. **Two overloads**: One without context (uses `NullContext.Default`), one with context
2. **Early returns**: Check for trivial cases (`Length <= 1`)
3. **Hybrid approach**: Use insertion sort for small subarrays
4. **AggressiveInlining**: Mark hot-path helper methods
5. **SortSpan operations**: Always use `s.Read()`, `s.Write()`, `s.Compare()`, `s.Swap()`
6. **Buffer IDs**: Use constants for buffer identification
7. **XML documentation**: Include time/space complexity and stability
