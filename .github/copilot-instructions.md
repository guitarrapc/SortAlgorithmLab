# SortAlgorithmLab - Project Instructions

## What is this project?

This is a C# sorting algorithm laboratory for educational and performance analysis purposes. It implements various sorting algorithms with comprehensive statistics tracking and visualization support.

**Tech Stack:** C# (.NET 9+), xUnit, BenchmarkDotNet

## Project Structure

- `src/SortLab.Core/` - Core sorting algorithms and interfaces
  - `Sortings/` - All sorting algorithm implementations
  - `Contexts/` - Statistics and visualization contexts
- `tests/SortLab.Tests/` - Unit tests for all algorithms
- `sandbox/` - Benchmark and experimental code
- `.github/agent_docs/` - Detailed implementation guidelines

## How to Work on This Project

### Running Tests

```powershell
dotnet test
```

### Running Benchmarks

```powershell
cd sandbox/SandboxBenchmark
dotnet run -c Release
```

### Building the Project

```powershell
dotnet build
```

## Important Guidelines

When implementing or reviewing sorting algorithms, refer to these detailed guides:

- **[Architecture](.github/agent_docs/architecture.md)** - Understand the Context + SortSpan pattern
- **[Performance Requirements](.github/agent_docs/performance_requirements.md)** - Zero-allocation, aggressive inlining, and memory management
- **[SortSpan Usage](.github/agent_docs/sortspan_usage.md)** - How to use SortSpan for all operations
- **[Implementation Template](.github/agent_docs/implementation_template.md)** - Template for new algorithms
- **[Coding Style](.github/agent_docs/coding_style.md)** - C# style conventions for this project

**Key Rule:** Always use `SortSpan<T>` methods (`Read`, `Write`, `Compare`, `Swap`) instead of direct array access. This ensures accurate statistics tracking.

## Progressive Disclosure

Before implementing a new sorting algorithm or making significant changes:

1. Read the relevant documentation files in `.github/agent_docs/`
2. Review existing similar implementations in `src/SortLab.Core/Sortings/`
3. Check corresponding tests in `tests/SortLab.Tests/`

Ask which documentation files you need if you're unsure what to read.
