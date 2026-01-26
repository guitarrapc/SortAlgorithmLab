using SortAlgorithm;
using System.Collections;

namespace SortLab.Tests;

public class MockPowerOfTwoReversedData : IEnumerable<object[]>
{
    private List<object[]> testData = new List<object[]>();

    public MockPowerOfTwoReversedData()
    {
        testData.Add([new InputSample<int>() { InputType = InputType.Reversed, Samples = Enumerable.Range(0, 16).Reverse().ToArray() }]);
        testData.Add([new InputSample<int>() { InputType = InputType.Reversed, Samples = Enumerable.Range(0, 64).Reverse().ToArray() }]);
        testData.Add([new InputSample<int>() { InputType = InputType.Reversed, Samples = Enumerable.Range(0, 256).Reverse().ToArray() }]);
    }

    public IEnumerator<object[]> GetEnumerator() => testData.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
