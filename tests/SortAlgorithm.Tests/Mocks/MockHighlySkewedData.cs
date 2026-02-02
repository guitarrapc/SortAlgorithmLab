namespace SortAlgorithm.Tests;

/// <summary>
/// Test data with highly skewed distribution where most elements have the same value.
/// Approximately 90% of elements are the same value, rest are unique.
/// This tests behavior when pivot selection encounters many duplicates.
/// </summary>
public static class MockHighlySkewedData
{
    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        var random = new Random(42);

        // Small array - 90% are value 1, 10% are random
        yield return () => new InputSample<int>()
        {
            InputType = InputType.HighlySkewed,
            Samples = Enumerable.Range(0, 100).Select(_ => random.Next(10) < 9 ? 1 : random.Next(100)).ToArray()
        };

        // Medium array - 90% are value 1, 10% are random
        yield return () => new InputSample<int>()
        {
            InputType = InputType.HighlySkewed,
            Samples = Enumerable.Range(0, 500).Select(_ => random.Next(10) < 9 ? 1 : random.Next(500)).ToArray()
        };

        // Large array - 90% are value 1, 10% are random
        yield return () => new InputSample<int>()
        {
            InputType = InputType.HighlySkewed,
            Samples = Enumerable.Range(0, 1000).Select(_ => random.Next(10) < 9 ? 1 : random.Next(1000)).ToArray()
        };

        // Very large array - 90% are value 1, 10% are random
        yield return () => new InputSample<int>()
        {
            InputType = InputType.HighlySkewed,
            Samples = Enumerable.Range(0, 10000).Select(_ => random.Next(10) < 9 ? 1 : random.Next(10000)).ToArray()
        };
    }
}
