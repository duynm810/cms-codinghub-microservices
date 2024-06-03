using WebApps.UI.Services.Interfaces;

namespace WebApps.UI.Services;

public class ErrorService : IErrorService
{
    public string GetErrorMessage(int statusCode)
    {
        return statusCode switch
        {
            404 => "Sorry, the resource you requested could not be found.",
            400 => "Bad request. Please check your request and try again.",
            500 => "Sorry, something went wrong on the server.",
            _ => "An unexpected error occurred."
        };
    }

    public string GetViewName(int statusCode)
    {
        return statusCode switch
        {
            404 => "HttpError-NotFound",
            400 => "HttpError-BadRequest",
            500 => "HttpError-InternalServerError",
            _ => "HttpError"
        };
    }
}