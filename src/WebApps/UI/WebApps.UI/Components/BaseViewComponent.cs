using Microsoft.AspNetCore.Mvc;
using WebApps.UI.Models.Commons;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Components;

public abstract class BaseViewComponent(ILogger logger) : ViewComponent
{
    protected IViewComponentResult HandleError(string methodName, int statusCode)
    {
        logger.Error("{MethodName} failed with status code {StatusCode}", methodName, statusCode);

        var items = new ErrorViewModel()
        {
            StatusCode = statusCode
        };

        return View("Error", items);
    }

    protected IViewComponentResult HandleException(string methodName, Exception e)
    {
        logger.Error(e, "{MethodName} encountered an exception", methodName);

        var items = new ErrorViewModel()
        {
            StatusCode = StatusCodes.Status500InternalServerError,
            StatusMessage = e.Message
        };

        return View("Error", items);
    }
}