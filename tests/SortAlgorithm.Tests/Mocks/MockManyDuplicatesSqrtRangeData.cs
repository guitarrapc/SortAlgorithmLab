using System.Collections;

namespace SortAlgorithm.Tests;

/// <summary>
/// Test data with many duplicates: random values between 0 and √n.
/// Based on BlockQuickSort paper benchmark: "random values between 0 and √n".
/// This creates approximately √n duplicates of each value on average.
/// </summary>
public class MockManyDuplicatesSqrtRangeData : IEnumerable<object[]>
{
    private readonly List<object[]> testData = new();

    public MockManyDuplicatesSqrtRangeData()
    {
        var random = new Random(42);

        // Small array - values in [0, √100) = [0, 10)
        testData.Add([new InputSample<int>() 
        { 
            InputType = InputType.ManyDuplicatesSqrtRange, 
            Samples = Enumerable.Range(0, 100).Select(_ => random.Next(10)).ToArray() 
        }]);

        // Medium array - values in [0, √500) ≈ [0, 22)
        testData.Add([new InputSample<int>() 
        { 
            InputType = InputType.ManyDuplicatesSqrtRange, 
            Samples = Enumerable.Range(0, 500).Select(_ => random.Next((int)Math.Sqrt(500))).ToArray() 
        }]);

        // Large array - values in [0, √1000) ≈ [0, 32)
        testData.Add([new InputSample<int>() 
        { 
            InputType = InputType.ManyDuplicatesSqrtRange, 
            Samples = Enumerable.Range(0, 1000).Select(_ => random.Next((int)Math.Sqrt(1000))).ToArray() 
        }]);

        // Very large array - values in [0, √10000) = [0, 100)
        testData.Add([new InputSample<int>() 
        { 
            InputType = InputType.ManyDuplicatesSqrtRange, 
            Samples = Enumerable.Range(0, 10000).Select(_ => random.Next(100)).ToArray() 
        }]);

        // Extra large array - values in [0, √25000) ≈ [0, 158)
        // This tests median-of-sqrt(n) pivot selection with many duplicates
        testData.Add([new InputSample<int>() 
        { 
            InputType = InputType.ManyDuplicatesSqrtRange, 
            Samples = Enumerable.Range(0, 25000).Select(_ => random.Next((int)Math.Sqrt(25000))).ToArray() 
        }]);
    }

    public IEnumerator<object[]> GetEnumerator() => testData.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
