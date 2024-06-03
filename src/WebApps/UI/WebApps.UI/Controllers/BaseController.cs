using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebApps.UI.CustomServices.Interfaces;
using WebApps.UI.Models.Commons;

namespace WebApps.UI.Controllers;

public abstract class BaseController(IErrorService errorService) : Controller
{
    protected IActionResult HandleError(HttpStatusCode statusCode)
    {
        string errorMessage = errorService.GetErrorMessage((int)statusCode);
        ViewData["ErrorMessage"] = errorMessage;
        return View(errorService.GetViewName((int)statusCode), new HttpErrorViewModel((int)statusCode));
    }
}