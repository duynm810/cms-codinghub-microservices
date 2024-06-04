using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Components;

public abstract class BaseViewComponent(IErrorService errorService, ILogger logger) : ViewComponent
{
    protected IViewComponentResult HandleError(HttpStatusCode statusCode, string methodName)
    {
        logger.Error("{MethodName} failed with status code {StatusCode}", methodName, statusCode);

        var errorMessage = errorService.GetErrorMessage((int)statusCode);
        ViewData["ErrorMessage"] = errorMessage;
        var viewName = errorService.GetViewName((int)statusCode);
        return View(viewName);
    }

    protected IViewComponentResult HandleException(Exception e, string methodName)
    {
        logger.Error(e, "{MethodName} encountered an exception", methodName);

        var errorMessage = errorService.GetErrorMessage((int)HttpStatusCode.InternalServerError);
        ViewData["ErrorMessage"] = errorMessage;
        return View(errorService.GetViewName((int)HttpStatusCode.InternalServerError));
    }
}