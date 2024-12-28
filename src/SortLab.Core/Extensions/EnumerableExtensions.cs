namespace SortLab.Core;

public static class EnumerableExtensions
{
    /// <summary>
    /// Returns a random sample of elements from the input sequence.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="sampleCount"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static IEnumerable<T> Sample<T>(this IEnumerable<T> source, int sampleCount)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sampleCount);

        return SampleCore(source, sampleCount, RandomUtil.ThreadRandom);
    }

    /// <summary>
    /// Returns a random sample of elements from the input sequence.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="sampleCount"></param>
    /// <param name="random"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static IEnumerable<T> Sample<T>(this IEnumerable<T> source, int sampleCount, Random random)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sampleCount);
        ArgumentNullException.ThrowIfNull(random);

        return SampleCore(source, sampleCount, random);
    }

    private static IEnumerable<T> SampleCore<T>(this IEnumerable<T> source, int sampleCount, Random random)
    {
        if (source is not IList<T> list)
        {
            list = source.ToList();
        }

        var len = list.Count;
        if (len == 0) yield break;

        for (int i = 0; i < sampleCount; i++)
        {
            var index = random.Next(0, len);
            yield return list[index];
        }
    }

    /// <summary>
    /// Returns a single random sample of elements from the input sequence.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static T SampleOne<T>(this IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return source.Sample(1).FirstOrDefault();
    }

    /// <summary>
    /// Returns a single random sample of elements from the input sequence.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="random"></param>
    /// <returns></returns>
    public static T SampleOne<T>(this IEnumerable<T> source, Random random)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(random);

        return source.Sample(1, random).FirstOrDefault();
    }
}
