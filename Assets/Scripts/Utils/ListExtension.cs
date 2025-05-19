using System;
using System.Collections.Generic;

public static class ListExtension
{
    public static T MinItem<T>(this IEnumerable<T> source, Func<T, float> selector)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (selector == null)
            throw new ArgumentNullException(nameof(selector));

        using (IEnumerator<T> enumerator = source.GetEnumerator())
        {
            if (enumerator.MoveNext() == false)
                return default;

            T minItem = enumerator.Current;
            float minValue = selector(minItem);

            while (enumerator.MoveNext())
            {
                T current = enumerator.Current;
                float currentValue = selector(current);

                if (currentValue < minValue)
                {
                    minValue = currentValue;
                    minItem = current;
                }
            }

            return minItem;
        }
    }
}