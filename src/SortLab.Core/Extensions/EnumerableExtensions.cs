using System;
using System.Collections.Generic;
using System.Linq;

namespace SortLab.Core;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Sample<T>(this IEnumerable<T> source, int sampleCount)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (sampleCount <= 0) throw new ArgumentOutOfRangeException(nameof(sampleCount));

        return SampleCore(source, sampleCount, RandomUtil.ThreadRandom);
    }

    public static IEnumerable<T> Sample<T>(this IEnumerable<T> source, int sampleCount, Random random)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (sampleCount <= 0) throw new ArgumentOutOfRangeException(nameof(sampleCount));
        if (random == null) throw new ArgumentNullException(nameof(random));

        return SampleCore(source, sampleCount, random);
    }

    static IEnumerable<T> SampleCore<T>(this IEnumerable<T> source, int sampleCount, Random random)
    {
        if (!(source is IList<T> list))
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

    public static T SampleOne<T>(this IEnumerable<T> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        return source.Sample(1).FirstOrDefault();
    }

    public static T SampleOne<T>(this IEnumerable<T> source, Random random)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (random == null) throw new ArgumentNullException(nameof(random));

        return source.Sample(1, random).FirstOrDefault();
    }
}
