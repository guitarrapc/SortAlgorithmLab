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
public class MySort<T> : SortBase<T> where T : IComparable<T>
{
    private const int InsertionSortThreshold = 16;

    public override void Sort(Span<T> span)
    {
        if (span.Length <= 1) return;
        if (span.Length <= InsertionSortThreshold)
        {
            InsertionSort(span);
            return;
        }
        SortCore(span, 0, span.Length - 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SortCore(Span<T> span, int left, int right)
    {
        // Use Index(), Compare(), Swap() consistently
    }
}
```

### Verification

- Test with BenchmarkDotNet
- Verify zero allocations in Release builds
- Ensure DEBUG tracking has no impact on Release performance

## Index Access Consistency Guidelines

When implementing sorting algorithms, maintain **consistent use of helper methods** for statistical tracking and code clarity.

This approach provides:

- **Accurate statistics** for algorithm analysis in DEBUG mode
- **Clean abstraction** for tracking operations
- **Zero performance impact** in production (Release builds)
- **Consistent code style** across all sorting implementations

### Required Practices

Use the following helper methods for all array/span operations, all methods are defined in the base sorting class `SortBase<T>`:

1. **Always use `Index(span, pos)` for all array/span access**
   - Both reads and writes must go through the `Index` method
   - This ensures accurate index access counting in DEBUG builds
   - Example:
     ```csharp
     // ✅ Correct - both sides use Index
     Index(span, i) = Index(span, j);

     // ❌ Incorrect - inconsistent access
     span[i] = span[j];
     ```

2. **Always use `Swap(ref a, ref b)` for element swapping**
   - Use the base class `Swap` method instead of tuple deconstruction
   - Ensures accurate swap counting in DEBUG builds
   - Example:
     ```csharp
     // ✅ Correct
     Swap(ref Index(span, i), ref Index(span, j));

     // ❌ Incorrect
     (span[i], span[j]) = (span[j], span[i]);
     ```

3. **Use `Compare(x, y)` for all comparisons**
   - Never call `CompareTo` directly
   - Ensures accurate comparison counting in DEBUG builds
    - Example:
      ```csharp
      // ✅ Correct
      if (Compare(Index(span, i), Index(span, j)) < 0) { ... }

      // ❌ Incorrect
      if (Index(span, i).CompareTo(Index(span, j)) < 0) { ... }
      ```

These methods are performance-optimized to have **no overhead** in Release builds.

- All helper methods are marked with `[MethodImpl(MethodImplOptions.AggressiveInlining)]`
- In Release builds, these methods are inlined with **zero overhead**
- Statistical tracking (`#if DEBUG`) is completely removed in Release builds
- There is **no performance penalty** for using these helper methods consistently
