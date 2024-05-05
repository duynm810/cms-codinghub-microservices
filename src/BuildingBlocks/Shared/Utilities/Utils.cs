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

    public static IEnumerable<string> GetExceptionList(this Exception ex)
    {
        if (ex.StackTrace == null)
        {
            return [];
        }

        var result = new List<string>() { ex.Message, ex.StackTrace };
        if (ex.InnerException == null)
        {
            return result;
        }

        result.Add(ex.InnerException.Message);
        if (ex.InnerException.StackTrace != null)
        {
            result.Add(ex.InnerException.StackTrace);
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
}