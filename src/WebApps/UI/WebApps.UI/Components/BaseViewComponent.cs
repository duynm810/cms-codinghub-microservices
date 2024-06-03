using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebApps.UI.Services.Interfaces;

namespace WebApps.UI.Components;

public abstract class BaseViewComponent(IErrorService errorService) : ViewComponent
{
    protected IViewComponentResult HandleError(HttpStatusCode statusCode)
    {
        var errorMessage = errorService.GetErrorMessage((int)statusCode);
        ViewData["ErrorMessage"] = errorMessage;
        var viewName = errorService.GetViewName((int)statusCode);
        return View(viewName);
    }

    protected IViewComponentResult HandleException(Exception ex, string methodName)
    {
        var errorMessage = errorService.GetErrorMessage((int)HttpStatusCode.InternalServerError);
        ViewData["ErrorMessage"] = errorMessage;
        return View(errorService.GetViewName((int)HttpStatusCode.InternalServerError));
    }
}