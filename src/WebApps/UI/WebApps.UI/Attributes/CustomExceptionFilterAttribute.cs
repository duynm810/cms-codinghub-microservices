using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApps.UI.Attributes;

/// <summary>
/// Catch and handle errors that are not caught in controllers (Bắt và xử lý các lỗi không được bắt trong các controller)
/// </summary>
public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        context.Result = new RedirectToActionResult("Error", "Error", null);
        context.ExceptionHandled = true;
    }
}