namespace SortAlgorithm.Tests;

/// <summary>
/// Test data with all identical elements.
/// Represents the worst-case scenario for partitioning algorithms.
/// Based on BlockQuickSort paper benchmark: constant arrays.
/// </summary>
public static class MockAllIdenticalData
{
    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        // Small array - all identical
        yield return () => new InputSample<int>()
        {
            InputType = InputType.AllIdentical,
            Samples = Enumerable.Repeat(42, 100).ToArray()
        };

        // Medium array - all identical
        yield return () => new InputSample<int>()
        {
            InputType = InputType.AllIdentical,
            Samples = Enumerable.Repeat(42, 500).ToArray()
        };

        // Large array - all identical
        yield return () => new InputSample<int>()
        {
            InputType = InputType.AllIdentical,
            Samples = Enumerable.Repeat(42, 1000).ToArray()
        };

        // Very large array - all identical
        yield return () => new InputSample<int>()
        {
            InputType = InputType.AllIdentical,
            Samples = Enumerable.Repeat(42, 10000).ToArray()
        };
    }
}
