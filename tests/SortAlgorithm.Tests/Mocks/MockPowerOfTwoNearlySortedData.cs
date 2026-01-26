using System.Collections;

namespace SortAlgorithm.Tests;

public class MockPowerOfTwoNearlySortedData : IEnumerable<object[]>
{
    private List<object[]> testData = new List<object[]>();

    public MockPowerOfTwoNearlySortedData()
    {
        testData.Add([new InputSample<int>() { InputType = InputType.Random, Samples = CreateNearlySorted(16) }]);
        testData.Add([new InputSample<int>() { InputType = InputType.Random, Samples = CreateNearlySorted(64) }]);
        testData.Add([new InputSample<int>() { InputType = InputType.Random, Samples = CreateNearlySorted(256) }]);
    }

    private static int[] CreateNearlySorted(int size)
    {
        var array = Enumerable.Range(0, size).ToArray();
        var random = new Random(42);
        // Swap a few elements to make it "nearly sorted"
        for (int i = 0; i < size / 10; i++)
        {
            int idx1 = random.Next(size);
            int idx2 = random.Next(size);
            (array[idx1], array[idx2]) = (array[idx2], array[idx1]);
        }
        return array;
    }

    public IEnumerator<object[]> GetEnumerator() => testData.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
