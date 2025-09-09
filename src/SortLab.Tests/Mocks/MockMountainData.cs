using System.Collections;

namespace SortLab.Tests;

public class MockMountainData : IEnumerable<object[]>
{
    private List<object[]> testData = new List<object[]>();

    public MockMountainData()
    {
        testData.Add([new InputSample<int>() { InputType = InputType.Mountain, Samples = Enumerable.Range(0, 50).Concat(Enumerable.Range(0, 50).Reverse()).ToArray() }]);
        testData.Add([new InputSample<int>() { InputType = InputType.Mountain, Samples = Enumerable.Range(0, 500).Concat(Enumerable.Range(0, 500).Reverse()).ToArray() }]);
        testData.Add([new InputSample<int>() { InputType = InputType.Mountain, Samples = Enumerable.Range(0, 5000).Concat(Enumerable.Range(0, 5000).Reverse()).ToArray() }]);
    }

    public IEnumerator<object[]> GetEnumerator() => testData.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
