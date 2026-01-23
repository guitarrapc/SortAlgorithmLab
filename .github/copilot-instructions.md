# Project Custom Instructions

## Coding Review Guidelines

When reviewing code, focus on the following points:

- Use of Modern C# Features
  - Consider using the latest syntax available (C# 13 or later), such as `using` declarations, file-scoped namespaces, and collection literals, etc.
- Coding Style
  - Follow general .NET coding guidelines for base conventions (e.g., naming rules).
    - Use `PascalCase` for constant names.
  - Inherit specific coding styles from existing code:
    - Do not use `_` or `s_` prefixes.
    - Omit the `private` modifier.
    - Prefer the use of `var`.
- Unit Tests
  - Check for the presence of unit tests.

Suggest fixes for any sections that deviate from these points.

## Implementation Guidelines

All sorting algorithms must be implemented with **maximum attention to performance and memory efficiency**.

### Architecture Overview

Sorting algorithms follow the **Class-based Context + SortSpan** pattern:

- **Static methods** - Sort algorithms are implemented as static methods (stateless)
- **ISortContext** - Handles observation (statistics, visualization) via callback interface
- **SortSpan<T>** - ref struct that wraps `Span<T>` + `ISortContext` for clean API

```
┌─────────────────────────────────────────────────────────────┐
│  BubbleSort.Sort<T>(span)                                   │
│  BubbleSort.Sort<T>(span, context)                          │
│  ─────────────────────────────────────────────────────────  │
│  • Static methods (no instance required)                    │
│  • Stateless (pure functions)                               │
│  • Context handles statistics/visualization                 │
└─────────────────────────────────────────────────────────────┘
```

### Core Requirements

1. **Zero Allocations**
   - Never allocate arrays or collections during sorting
   - Use `Span<T>` and `stackalloc` for temporary storage
   - For large buffers, consider `ArrayPool<T>.Shared`

2. **Aggressive Inlining**
   - Mark hot-path methods with `[MethodImpl(MethodImplOptions.AggressiveInlining)]`
   - Especially for methods called frequently in loops

3. **Loop Optimization**
   - Cache frequently accessed values outside loops
   - Use `for` loops with indices instead of `foreach`
   - Minimize redundant comparisons

4. **Hybrid Approaches**
   - Use insertion sort for small subarrays (typically < 16-32 elements)
   - Switch algorithms based on data characteristics when beneficial

### Example Pattern

```csharp
public static class MySort
{
    private const int InsertionSortThreshold = 16;

    public static void Sort<T>(Span<T> span) where T : IComparable<T>
        => Sort(span, NullContext.Default);

    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T>(span, context);

        if (s.Length <= InsertionSortThreshold)
        {
            InsertionSort(s);
            return;
        }
        SortCore(s, 0, s.Length - 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SortCore<T>(SortSpan<T> s, int left, int right) where T : IComparable<T>
    {
        // Use s.Read(), s.Write(), s.Compare(), s.Swap() consistently
    }
}
```

### Verification

- Test with BenchmarkDotNet
- Verify zero allocations in Release builds
- Ensure Context overhead is minimal with `NullContext.Default`

## SortSpan Usage Guidelines

When implementing sorting algorithms, use **SortSpan<T>** for all array/span operations.

This approach provides:

- **Accurate statistics** for algorithm analysis via ISortContext
- **Clean abstraction** for tracking operations
- **Minimal performance impact** with NullContext (empty method calls)
- **Consistent code style** across all sorting implementations
- **Separation of concerns** - algorithm logic vs observation

### Required Practices

Use `SortSpan<T>` helper methods for all array/span operations:

1. **Use `s.Read(i)` for reading elements**
   - Notifies context via `OnIndexRead(i)`
   - Example:
     ```csharp
     var s = new SortSpan<T>(span, context);

     // ✅ Correct - uses Read
     var value = s.Read(i);

     // ❌ Incorrect - bypasses context
     var value = span[i];
     ```

2. **Use `s.Write(i, value)` for writing elements**
   - Notifies context via `OnIndexWrite(i)`
   - Example:
     ```csharp
     // ✅ Correct - uses Write
     s.Write(i, value);

     // ❌ Incorrect - bypasses context
     span[i] = value;
     ```

3. **Use `s.Compare(i, j)` for comparisons**
   - Reads both elements, compares, and notifies context via `OnCompare(i, j, result)`
   - Example:
     ```csharp
     // ✅ Correct - comparing two indices
     if (s.Compare(i, j) < 0) { ... }

     // ❌ Incorrect - bypasses context
     if (span[i].CompareTo(span[j]) < 0) { ... }
     ```

   - For comparing with a value (not an index):
     ```csharp
     var value = s.Read(someIndex);

     // ✅ Correct - comparing index with value
     if (s.Compare(i, value) < 0) { ... }

     // ✅ Correct - comparing value with index
     if (s.Compare(value, i) < 0) { ... }

     // ❌ Incorrect - direct CompareTo bypasses context
     if (s.Read(i).CompareTo(value) < 0) { ... }
     if (value.CompareTo(s.Read(i)) < 0) { ... }
     ```

4. **Use `s.Swap(i, j)` for element swapping**
   - Reads both elements, notifies context via `OnSwap(i, j)`, then writes
   - Example:
     ```csharp
     // ✅ Correct
     s.Swap(i, j);

     // ❌ Incorrect - bypasses context
     (span[i], span[j]) = (span[j], span[i]);
     ```

**Important:** Never use `CompareTo()` directly. All comparisons must go through `SortSpan` methods to ensure accurate statistics tracking.

### Context Types

| Context | Purpose | Overhead |
|---------|---------|----------|
| `NullContext.Default` | No statistics (production) | Minimal (empty methods) |
| `StatisticsContext` | Collect operation counts | Small (Interlocked.Increment) |
| `VisualizationContext` | Animation/rendering callbacks | Medium (callback invocation) |
| `CompositeSortContext` | Combine multiple contexts | Medium-Large |

### Usage Examples

```csharp
// Production - no statistics
MySort.Sort<int>(array);

// With statistics
var stats = new StatisticsContext();
MySort.Sort(array.AsSpan(), stats);
Console.WriteLine($"Compares: {stats.CompareCount}, Swaps: {stats.SwapCount}");
Console.WriteLine($"Reads: {stats.IndexReadCount}, Writes: {stats.IndexWriteCount}");

// With visualization
var viz = new VisualizationContext(
    onSwap: (i, j) => RenderSwap(i, j),
    onCompare: (i, j, result) => HighlightCompare(i, j)
);
MySort.Sort(array.AsSpan(), viz);

// Combined statistics + visualization
var composite = new CompositeSortContext(stats, viz);
MySort.Sort(array.AsSpan(), composite);
```
