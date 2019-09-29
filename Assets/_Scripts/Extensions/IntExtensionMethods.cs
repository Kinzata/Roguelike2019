
using System.Collections.Generic;

public static class IntExtensionMethods
{
    public static IEnumerable<int> To(this int from, int to)
    {
        if (from < to)
        {
            while (from <= to)
            {
                yield return from++;
            }
        }
        else
        {
            while (from >= to)
            {
                yield return from--;
            }
        }
    }

}