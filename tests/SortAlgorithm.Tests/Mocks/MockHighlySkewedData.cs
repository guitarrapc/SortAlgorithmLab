using System.Collections;

namespace SortAlgorithm.Tests;

/// <summary>
/// Test data with highly skewed distribution where most elements have the same value.
/// Approximately 90% of elements are the same value, rest are unique.
/// This tests behavior when pivot selection encounters many duplicates.
/// </summary>
public class MockHighlySkewedData : IEnumerable<object[]>
{
    private readonly List<object[]> testData = new();

    public MockHighlySkewedData()
    {
        var random = new Random(42);

        // Small array - 90% are value 1, 10% are random
        testData.Add([new InputSample<int>() 
        { 
            InputType = InputType.HighlySkewed, 
            Samples = Enumerable.Range(0, 100).Select(_ => random.Next(10) < 9 ? 1 : random.Next(100)).ToArray() 
        }]);

        // Medium array - 90% are value 1, 10% are random
        testData.Add([new InputSample<int>() 
        { 
            InputType = InputType.HighlySkewed, 
            Samples = Enumerable.Range(0, 500).Select(_ => random.Next(10) < 9 ? 1 : random.Next(500)).ToArray() 
        }]);

        // Large array - 90% are value 1, 10% are random
        testData.Add([new InputSample<int>() 
        { 
            InputType = InputType.HighlySkewed, 
            Samples = Enumerable.Range(0, 1000).Select(_ => random.Next(10) < 9 ? 1 : random.Next(1000)).ToArray() 
        }]);

        // Very large array - 90% are value 1, 10% are random
        testData.Add([new InputSample<int>() 
        { 
            InputType = InputType.HighlySkewed, 
            Samples = Enumerable.Range(0, 10000).Select(_ => random.Next(10) < 9 ? 1 : random.Next(10000)).ToArray() 
        }]);
    }

    public IEnumerator<object[]> GetEnumerator() => testData.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
