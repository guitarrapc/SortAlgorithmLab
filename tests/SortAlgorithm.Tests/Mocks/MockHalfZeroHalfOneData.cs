using System.Collections;

namespace SortAlgorithm.Tests;

/// <summary>
/// Test data with first half all 0s, second half all 1s.
/// Based on BlockQuickSort paper benchmark: "A[i] = 0 for i &lt; n/2 and A[i] = 1 otherwise".
/// This tests partitioning on pre-sorted binary data.
/// </summary>
public class MockHalfZeroHalfOneData : IEnumerable<object[]>
{
    private readonly List<object[]> testData = new();

    public MockHalfZeroHalfOneData()
    {
        // Small array - first half 0s, second half 1s
        testData.Add([new InputSample<int>() 
        { 
            InputType = InputType.HalfZeroHalfOne, 
            Samples = Enumerable.Range(0, 100).Select(i => i < 50 ? 0 : 1).ToArray() 
        }]);

        // Medium array - first half 0s, second half 1s (power of 2 boundary)
        testData.Add([new InputSample<int>() 
        { 
            InputType = InputType.HalfZeroHalfOne, 
            Samples = Enumerable.Range(0, 256).Select(i => i < 128 ? 0 : 1).ToArray() 
        }]);

        // Large array - first half 0s, second half 1s
        testData.Add([new InputSample<int>() 
        { 
            InputType = InputType.HalfZeroHalfOne, 
            Samples = Enumerable.Range(0, 1000).Select(i => i < 500 ? 0 : 1).ToArray() 
        }]);

        // Very large array - first half 0s, second half 1s
        testData.Add([new InputSample<int>() 
        { 
            InputType = InputType.HalfZeroHalfOne, 
            Samples = Enumerable.Range(0, 10000).Select(i => i < 5000 ? 0 : 1).ToArray() 
        }]);
    }

    public IEnumerator<object[]> GetEnumerator() => testData.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
