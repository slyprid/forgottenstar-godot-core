// ReSharper disable CheckNamespace
using System;
using System.Collections.Generic;

public static class ListExtensions
{
    public static int MaxIndex<T>(this IEnumerable<T> source)
    {
        IComparer<T> comparer = Comparer<T>.Default;
        using var iterator = source.GetEnumerator();
        if (!iterator.MoveNext())
        {
            throw new InvalidOperationException("Empty sequence");
        }
        var maxIndex = 0;
        var maxElement = iterator.Current;
        var index = 0;
        while (iterator.MoveNext())
        {
            index++;
            var element = iterator.Current;
            if (comparer.Compare(element, maxElement) <= 0) continue;
            maxElement = element;
            maxIndex = index;
        }
        return maxIndex;
    }
}