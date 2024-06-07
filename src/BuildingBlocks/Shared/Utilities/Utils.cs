using System.Text;
using System.Text.RegularExpressions;

namespace Shared.Utilities;

public static class Utils
{
    public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> list)
    {
        var enumerable = list.ToList();
        return enumerable.Count != 0 && enumerable.All(x => x != null);
    }

    public static IEnumerable<string> GetExceptionList(this Exception e)
    {
        if (e.StackTrace == null)
        {
            return [];
        }

        var result = new List<string>() { e.Message, e.StackTrace };
        if (e.InnerException == null)
        {
            return result;
        }

        result.Add(e.InnerException.Message);
        if (e.InnerException.StackTrace != null)
        {
            result.Add(e.InnerException.StackTrace);
        }

        return result;
    }

    public static string ToUnSignString(string input)
    {
        input = input.Trim();
        for (var i = 0x20; i < 0x30; i++) input = input.Replace(((char)i).ToString(), " ");
        input = input.Replace(".", "-");
        input = input.Replace(" ", "-");
        input = input.Replace(",", "-");
        input = input.Replace(";", "-");
        input = input.Replace(":", "-");
        input = input.Replace("  ", "-");
        var regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
        var str = input.Normalize(NormalizationForm.FormD);
        var str2 = regex.Replace(str, string.Empty).Replace('đ', 'd').Replace('Đ', 'D');
        while (str2.IndexOf("?", StringComparison.Ordinal) >= 0)
            str2 = str2.Remove(str2.IndexOf("?", StringComparison.Ordinal), 1);
        while (str2.Contains("--")) str2 = str2.Replace("--", "-").ToLower();
        return str2.ToLower();
    }
    
    public static bool ContainsId(this string list, string id)
    {
        return !string.IsNullOrEmpty(list) && list.Split(',').Contains(id);
    }
}