using System.Collections;

namespace SortAlgorithm.Tests;

/// <summary>
/// Test data with all identical elements.
/// Represents the worst-case scenario for partitioning algorithms.
/// Based on BlockQuickSort paper benchmark: constant arrays.
/// </summary>
public class MockAllIdenticalData : IEnumerable<object[]>
{
    private readonly List<object[]> testData = new();

    public MockAllIdenticalData()
    {
        // Small array - all identical
        testData.Add([new InputSample<int>() 
        { 
            InputType = InputType.AllIdentical, 
            Samples = Enumerable.Repeat(42, 100).ToArray() 
        }]);

        // Medium array - all identical
        testData.Add([new InputSample<int>() 
        { 
            InputType = InputType.AllIdentical, 
            Samples = Enumerable.Repeat(42, 500).ToArray() 
        }]);

        // Large array - all identical
        testData.Add([new InputSample<int>() 
        { 
            InputType = InputType.AllIdentical, 
            Samples = Enumerable.Repeat(42, 1000).ToArray() 
        }]);

        // Very large array - all identical
        testData.Add([new InputSample<int>() 
        { 
            InputType = InputType.AllIdentical, 
            Samples = Enumerable.Repeat(42, 10000).ToArray() 
        }]);
    }

    public IEnumerator<object[]> GetEnumerator() => testData.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
