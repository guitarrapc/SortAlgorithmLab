# SortAlgorithmLab - Project Instructions

## What is this project?

This is a C# sorting algorithm laboratory for educational and performance analysis purposes. It implements various sorting algorithms with comprehensive statistics tracking and visualization support.

**Tech Stack:** C# (.NET 10+), xUnit, BenchmarkDotNet

## Project Structure

- `src/SortAlgorithm/` - Core sorting algorithms and interfaces
  - `Algorithms/` - Sorting algorithm implementations
  - `Contexts/` - Statistics and visualization contexts
- `tests/SortAlgorithm.Tests/` - Unit tests for all algorithms
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

### Run Some Script

If you needd create some .cs file to verify somthing, you can create it in the `sandbox` folder and run it via:

```powershell
cd sandbox
dotnet run -c Release --project YourScriptProjectName.csproj
```

or you can directly run a single C# file:

```powershell
dotnet run YourScriptFile.cs
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
2. Review existing similar implementations in `src/SortAlgorithm/Algorithms/`
3. Check corresponding tests in `tests/SortAlgorithm.Tests/`

Ask which documentation files you need if you're unsure what to read.
