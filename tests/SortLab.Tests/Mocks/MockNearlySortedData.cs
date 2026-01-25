using SortLab.Core;
using System.Collections;

namespace SortLab.Tests;

public class MockNearlySortedData : IEnumerable<object[]>
{
    private List<object[]> testData = new List<object[]>();

    public MockNearlySortedData()
    {
        testData.Add([new InputSample<int>() { InputType = InputType.NearlySorted, Samples = Enumerable.Range(0, 90).Concat(Enumerable.Range(0, 100).Sample(10)).ToArray() }]);
        testData.Add([new InputSample<int>() { InputType = InputType.NearlySorted, Samples = Enumerable.Range(0, 990).Concat(Enumerable.Range(0, 1000).Sample(10)).ToArray() }]);
        testData.Add([new InputSample<int>() { InputType = InputType.NearlySorted, Samples = Enumerable.Range(0, 9990).Concat(Enumerable.Range(0, 10000).Sample(10)).ToArray() }]);
    }

    public IEnumerator<object[]> GetEnumerator() => testData.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
