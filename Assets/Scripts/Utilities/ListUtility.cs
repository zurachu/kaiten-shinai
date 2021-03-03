using System;
using System.Collections.Generic;
using System.Linq;

public static class ListUtility
{
    public static bool IsNullOrEmpty<T>(List<T> list)
    {
        return list == null || list.Count == 0;
    }

    public static bool IsIndexInside<T>(List<T> list, int index)
    {
        return !IsNullOrEmpty(list) && 0 <= index && index < list.Count;
    }

    public static bool TryGetValue<T>(List<T> list, int index, out T value)
    {
        if (!IsIndexInside(list, index))
        {
            value = default;
            return false;
        }

        value = list[index];
        return true;
    }

    public static List<T> Shuffle<T>(List<T> list)
    {
        return list.OrderBy(i => Guid.NewGuid()).ToList();
    }
}
