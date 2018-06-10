using System;

namespace SortAlgorithm
{
    public interface ISort<T> where T : IComparable<T>
    {
        SortType SortType { get; }

        IStatistics Statistics { get; }
        T[] Sort(T[] array);
    }
}
