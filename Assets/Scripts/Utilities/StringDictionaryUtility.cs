using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

public static class StringDictionaryUtility
{
    public static string Dump(Dictionary<string, string> dictionary)
    {
        if (dictionary == null)
        {
            return string.Empty;
        }

        var stringBuilder = new StringBuilder();
        foreach (var item in dictionary)
        {
            stringBuilder.Append($"{item.Key}:{item.Value}\n");
        }

        return stringBuilder.ToString();
    }

    public static int GetInt(Dictionary<string, string> dictionary, string key)
    {
        return GetValue<int>(dictionary, key);
    }

    public static float GetFloat(Dictionary<string, string> dictionary, string key)
    {
        return GetValue<float>(dictionary, key);
    }

    public static string GetString(Dictionary<string, string> dictionary, string key)
    {
        return GetValue<string>(dictionary, key);
    }

    private static T GetValue<T>(Dictionary<string, string> dictionary, string key)
    {
        if (dictionary != null)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter != null)
                {
                    return (T)converter.ConvertFromString(value);
                }
            }
        }

        return default;
    }
}
