namespace Shared.Responses;

public class ApiResult<T> : BaseApiResult
{
    public T? Data { get; set; }

    // Default constructor
    public ApiResult()
    {
        Messages = new List<string>();
        IsSuccess = false;
    }

    // Constructor used for successes where data is returned
    private ApiResult(T? data, string title = "Success")
    {
        IsSuccess = true;
        StatusCode = 200;
        Data = data;
        Title = title;
    }

    // Constructor used for failures where no data is to be returned
    private ApiResult(int statusCode, List<string> messages, string title = "Error")
    {
        IsSuccess = false;
        StatusCode = statusCode;
        Messages = messages;
        Data = default;
        Title = title;
    }

    // Static helper method for success
    public ApiResult<T?> Success(T data)
    {
        return new ApiResult<T?>(data);
    }

    // Static helper method for failure
    public ApiResult<T> Failure(int statusCode, List<string> messages, string title = "Error")
    {
        return new ApiResult<T>(statusCode, messages, title);
    }
}