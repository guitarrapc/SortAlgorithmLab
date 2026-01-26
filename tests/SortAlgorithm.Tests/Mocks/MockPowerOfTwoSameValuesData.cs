using System.Collections;

namespace SortAlgorithm.Tests;

public class MockPowerOfTwoSameValuesData : IEnumerable<object[]>
{
    private List<object[]> testData = new List<object[]>();

    public MockPowerOfTwoSameValuesData()
    {
        testData.Add([new InputSample<int>() { InputType = InputType.Random, Samples = Enumerable.Repeat(42, 16).ToArray() }]);
        testData.Add([new InputSample<int>() { InputType = InputType.Random, Samples = Enumerable.Repeat(42, 64).ToArray() }]);
        testData.Add([new InputSample<int>() { InputType = InputType.Random, Samples = Enumerable.Repeat(42, 256).ToArray() }]);
    }

    public IEnumerator<object[]> GetEnumerator() => testData.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
