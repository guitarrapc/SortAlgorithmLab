namespace SortLab.Tests;

public enum InputType
{
    Random,
    Reversed,
    Mountain,
    NearlySorted,
    Sorted,
    SameValues,
    Stability,
    AntiQuickSort,

    MixRandom,
    NegativeRandom,
    DictionaryRamdom,
}

public interface IInputSample<T> where T : IComparable
{
    InputType InputType { get; set; }
    T[] Samples { get; set; }
    CustomKeyValuePair<T, string>[] DictionarySamples { get; set; }
}

public class InputSample<T> : IInputSample<T> where T : IComparable
{
    public required InputType InputType { get; set; }
    public T[] Samples { get; set; } = [];
    public CustomKeyValuePair<T, string>[] DictionarySamples { get; set; } = [];
}

public readonly struct CustomKeyValuePair<TKey, TValue> : IComparable<CustomKeyValuePair<TKey, TValue>> where TKey : notnull, IComparable
{
    public CustomKeyValuePair(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
    public TKey Key { get; }
    public TValue Value { get; }

    public int CompareTo(CustomKeyValuePair<TKey, TValue> other)
    {
        return Key.CompareTo(other.Key);
    }
}
