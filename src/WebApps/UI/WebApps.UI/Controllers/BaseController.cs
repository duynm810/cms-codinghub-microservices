using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebApps.UI.Models.Commons;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Controllers;

public abstract class BaseController(IErrorService errorService, ILogger logger) : Controller
{
    protected IActionResult HandleError(HttpStatusCode statusCode, string methodName)
    {
        logger.Error("{MethodName} failed with status code {StatusCode}", methodName, statusCode);
        
        var errorMessage = errorService.GetErrorMessage((int)statusCode);
        ViewData["ErrorMessage"] = errorMessage;
        return View(errorService.GetViewName((int)statusCode), new HttpErrorViewModel((int)statusCode));
    }

    protected IActionResult HandleException(Exception e, string methodName)
    {
        logger.Error(e, "{MethodName} encountered an exception", methodName);
        
        var errorMessage = errorService.GetErrorMessage((int)HttpStatusCode.InternalServerError);
        ViewData["ErrorMessage"] = errorMessage;
        return View(errorService.GetViewName((int)HttpStatusCode.InternalServerError));
    }
}