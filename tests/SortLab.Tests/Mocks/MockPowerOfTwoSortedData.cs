using System.Collections;

namespace SortLab.Tests;

public class MockPowerOfTwoSortedData : IEnumerable<object[]>
{
    private List<object[]> testData = new List<object[]>();

    public MockPowerOfTwoSortedData()
    {
        testData.Add([new InputSample<int>() { InputType = InputType.Sorted, Samples = Enumerable.Range(0, 16).ToArray() }]);
        testData.Add([new InputSample<int>() { InputType = InputType.Sorted, Samples = Enumerable.Range(0, 64).ToArray() }]);
        testData.Add([new InputSample<int>() { InputType = InputType.Sorted, Samples = Enumerable.Range(0, 256).ToArray() }]);
    }

    public IEnumerator<object[]> GetEnumerator() => testData.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
