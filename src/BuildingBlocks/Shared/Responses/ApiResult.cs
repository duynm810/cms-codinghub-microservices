namespace Shared.Responses;

public class ApiResult<T> : BaseApiResult
{
    public T? Data { get; set; }

    // Default constructor
    public ApiResult()
    {
        Messages = [];
        IsSuccess = false;
    }

    // Static helper method for success
    public void Success(T data, string title = "Success")
    {
        IsSuccess = true;
        StatusCode = 200;
        Data = data;
        Title = title;
    }

    // Static helper method for failure
    public void Failure(int statusCode, List<string> messages, string title = "Failed")
    {
        IsSuccess = false;
        StatusCode = statusCode;
        Messages = messages;
        Data = default;
        Title = title;
    }

    public void Error(string message, string title = "Error")
    {
        IsSuccess = false;
        Messages = [message];
        Data = default(T);
        Title = title;
    }
}