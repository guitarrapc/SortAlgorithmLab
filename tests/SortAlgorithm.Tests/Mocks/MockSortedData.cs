using System.Collections;

namespace SortAlgorithm.Tests;

public class MockSortedData : IEnumerable<object[]>
{
    private List<object[]> testData = new List<object[]>();

    public MockSortedData()
    {
        testData.Add([new InputSample<int>() { InputType = InputType.Sorted, Samples = Enumerable.Range(0, 100).ToArray() }]);
        testData.Add([new InputSample<int>() { InputType = InputType.Sorted, Samples = Enumerable.Range(0, 1000).ToArray() }]);
        testData.Add([new InputSample<int>() { InputType = InputType.Sorted, Samples = Enumerable.Range(0, 10000).ToArray() }]);
    }

    public IEnumerator<object[]> GetEnumerator() => testData.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
