namespace SortLab.Core;

public interface ISort<T> where T : IComparable<T>
{
    SortMethod SortType { get; }

    IStatistics Statistics { get; }
    T[] Sort(T[] array);
}
