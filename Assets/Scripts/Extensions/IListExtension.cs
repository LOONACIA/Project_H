using System;
using System.Collections.Generic;

public static class IListExtension
{
    public static void RemoveAll<T>(this IList<T> list, Predicate<T> match)
    {
        if (list is List<T> li)
        {
            li.RemoveAll(match);
            return;
        }
        
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (match(list[i]))
            {
                list.RemoveAt(i);
            }
        }
    }
}