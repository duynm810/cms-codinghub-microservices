using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApps.UI.Models.Commons;
using WebApps.UI.Services.Interfaces;

namespace WebApps.UI.Controllers;

/// <summary>
/// Handle global errors and HTTP status codes (Xử lý các lỗi toàn cục và các mã trạng thái HTTP)
/// </summary>
public class ErrorController(IErrorService errorService) : Controller
{
    [Route("HttpError/{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
        var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

        var viewName = errorService.GetViewName(statusCode);
        var errorMessage = errorService.GetErrorMessage(statusCode);

        ViewData["ErrorMessage"] = errorMessage;

        if (statusCodeResult != null)
        {
            ViewData["OriginalPath"] = statusCodeResult.OriginalPath;
            ViewData["OriginalQueryString"] = statusCodeResult.OriginalQueryString;
        }

        return View(viewName, new HttpErrorViewModel(statusCode));
    }

    [Route("HttpError")]
    public IActionResult Error()
    {
        var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var exception = feature?.Error;

        ViewData["ErrorMessage"] = exception?.Message ?? errorService.GetErrorMessage((int)HttpStatusCode.InternalServerError);
        return View(errorService.GetViewName((int)HttpStatusCode.InternalServerError), new HttpErrorViewModel((int)HttpStatusCode.InternalServerError));
    }
}