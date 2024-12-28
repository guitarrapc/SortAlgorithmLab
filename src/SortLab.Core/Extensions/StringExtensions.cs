using System;
using System.Collections.Generic;

namespace SortLab.Core;

public static class StringExtensions
{
    /// <summary>
    /// Concat string arrays into single string.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static string ToJoinedString<T>(this IEnumerable<T> source, string separator = "")
    {
        return String.Join(separator, source);
    }
}
