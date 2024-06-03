using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApps.UI.Models.Commons;

namespace WebApps.UI.Controllers;

public class ErrorController : Controller
{
    [Route("Error/{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
        var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

        string viewName = statusCode switch
        {
            404 => "HttpError-NotFound",
            400 => "HttpError-BadRequest",
            500 => "HttpError-InternalServerError",
            _ => "Error"
        };

        switch (statusCode)
        {
            case 404:
                ViewData["ErrorMessage"] = "Sorry, the resource you requested could not be found.";
                break;
            case 400:
                ViewData["ErrorMessage"] = "Bad request. Please check your request and try again.";
                break;
            case 500:
                ViewData["ErrorMessage"] = "Sorry, something went wrong on the server.";
                break;
            default:
                ViewData["ErrorMessage"] = "An unexpected error occurred.";
                break;
        }

        // Optionally log or use statusCodeResult for debugging
        if (statusCodeResult != null)
        {
            ViewData["OriginalPath"] = statusCodeResult.OriginalPath;
            ViewData["OriginalQueryString"] = statusCodeResult.OriginalQueryString;
        }

        return View(viewName, new HttpErrorViewModel(statusCode));
    }

    [Route("Error")]
    public IActionResult Error()
    {
        var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var exception = feature?.Error;

        ViewData["ErrorMessage"] = exception?.Message ?? "An unexpected error occurred.";
        return View(new HttpErrorViewModel((int)HttpStatusCode.InternalServerError));
    }
}