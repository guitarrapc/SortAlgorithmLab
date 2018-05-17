using System;

namespace SortAlgorithm
{
    public interface ISort<T> where T : IComparable<T>
    {
        SortStatics SortStatics { get; }
        T[] Sort(T[] array);
    }
}
