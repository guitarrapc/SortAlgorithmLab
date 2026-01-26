# Sorting Algorithm Architecture

## Design Pattern: Class-based Context + SortSpan

Sorting algorithms follow a consistent architecture:

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

## File Locations

- Algorithm implementations: [src/SortAlgorithm/Sortings/](../../src/SortAlgorithm/Sortings/)
- Core interfaces: [src/SortAlgorithm/](../../src/SortAlgorithm/)
- Unit tests: [tests/SortAlgorithm.Tests/](../../tests/SortAlgorithm.Tests/)
- Benchmark code: [sandbox/SandboxBenchmark/](../../sandbox/SandboxBenchmark/)

## Context Types

| Context | Purpose | Overhead |
|---------|---------|----------|
| `NullContext.Default` | No statistics (production) | Minimal (empty methods) |
| `StatisticsContext` | Collect operation counts | Small (Interlocked.Increment) |
| `VisualizationContext` | Animation/rendering callbacks | Medium (callback invocation) |
| `CompositeSortContext` | Combine multiple contexts | Medium-Large |
