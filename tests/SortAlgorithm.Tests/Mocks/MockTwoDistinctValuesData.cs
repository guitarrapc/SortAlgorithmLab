using System.Collections;

namespace SortAlgorithm.Tests;

/// <summary>
/// Test data with only two distinct values distributed randomly.
/// Based on BlockQuickSort paper benchmark: "random 0-1 values".
/// This tests partitioning behavior with binary data.
/// </summary>
public class MockTwoDistinctValuesData : IEnumerable<object[]>
{
    private readonly List<object[]> testData = new();

    public MockTwoDistinctValuesData()
    {
        var random = new Random(42);

        // Small array - random 0-1
        testData.Add([new InputSample<int>() 
        { 
            InputType = InputType.TwoDistinctValues, 
            Samples = Enumerable.Range(0, 100).Select(_ => random.Next(2)).ToArray() 
        }]);

        // Medium array - random 0-1
        testData.Add([new InputSample<int>() 
        { 
            InputType = InputType.TwoDistinctValues, 
            Samples = Enumerable.Range(0, 500).Select(_ => random.Next(2)).ToArray() 
        }]);

        // Large array - random 0-1
        testData.Add([new InputSample<int>() 
        { 
            InputType = InputType.TwoDistinctValues, 
            Samples = Enumerable.Range(0, 1000).Select(_ => random.Next(2)).ToArray() 
        }]);

        // Very large array - random 0-1
        testData.Add([new InputSample<int>() 
        { 
            InputType = InputType.TwoDistinctValues, 
            Samples = Enumerable.Range(0, 5000).Select(_ => random.Next(2)).ToArray() 
        }]);

        // Extra large array - random 0-1
        testData.Add([new InputSample<int>() 
        { 
            InputType = InputType.TwoDistinctValues, 
            Samples = Enumerable.Range(0, 10000).Select(_ => random.Next(2)).ToArray() 
        }]);
    }

    public IEnumerator<object[]> GetEnumerator() => testData.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
