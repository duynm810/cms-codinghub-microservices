using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebApps.UI.Models.Commons;
using WebApps.UI.Services.Interfaces;

namespace WebApps.UI.Controllers;

public abstract class BaseController(IErrorService errorService) : Controller
{
    protected IActionResult HandleError(HttpStatusCode statusCode)
    {
        var errorMessage = errorService.GetErrorMessage((int)statusCode);
        ViewData["ErrorMessage"] = errorMessage;
        return View(errorService.GetViewName((int)statusCode), new HttpErrorViewModel((int)statusCode));
    }

    protected IActionResult HandleException(Exception ex, string methodName)
    {
        var errorMessage = errorService.GetErrorMessage((int)HttpStatusCode.InternalServerError);
        ViewData["ErrorMessage"] = errorMessage;
        return View(errorService.GetViewName((int)HttpStatusCode.InternalServerError));
    }
}