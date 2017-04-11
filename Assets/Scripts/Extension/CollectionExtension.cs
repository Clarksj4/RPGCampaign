using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class CollectionExtension
{
    public static T Find<T>(this Queue<T> queue, Predicate<T> fn)
    {
        foreach (var item in queue)
        {
            if (fn(item))
                return item;
        }

        return default(T);    
    }
}
