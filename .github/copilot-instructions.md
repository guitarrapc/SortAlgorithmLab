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
