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
}