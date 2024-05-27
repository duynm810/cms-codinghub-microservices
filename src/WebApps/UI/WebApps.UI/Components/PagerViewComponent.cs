using Microsoft.AspNetCore.Mvc;
using Shared.SeedWorks;

namespace WebApps.UI.Components;

using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Web;

public class PagerViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(MetaData metaData)
    {
        // Lấy tên hành động và controller hiện tại
        var actionName = ViewContext.RouteData.Values["action"]?.ToString();
        var controllerName = ViewContext.RouteData.Values["controller"]?.ToString();

        if (actionName == null || controllerName == null)
        {
            throw new InvalidOperationException("Action name or controller name cannot be null.");
        }

        // Xây dựng urlTemplate rõ ràng
        var urlTemplate = new StringBuilder(Url.Action(actionName, controllerName) + "?page={0}");
        var queryParameters = HttpUtility.ParseQueryString(HttpContext.Request.QueryString.ToString());

        foreach (var key in queryParameters.AllKeys)
        {
            if (key == "page") continue;
            var value = queryParameters[key];
            if (value != null)
            {
                urlTemplate.Append($"&{key}={Uri.EscapeDataString(value)}");
            }
        }

        ViewBag.UrlTemplate = urlTemplate.ToString();
        return await Task.FromResult(View(metaData));
    }
}
