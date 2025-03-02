using Shared.SeedWorks;

namespace WebApps.UI.Components;

using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Web;

[ViewComponent(Name = "Pager")]
public class PagerViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(MetaData metaData)
    {
        var actionName = ViewContext.RouteData.Values["action"]?.ToString();
        var controllerName = ViewContext.RouteData.Values["controller"]?.ToString();

        if (actionName == null || controllerName == null)
        {
            throw new InvalidOperationException("Action name or controller name cannot be null.");
        }

        // Xây dựng urlTemplate rõ ràng
        var urlTemplate = new StringBuilder();

        // Kiểm tra nếu là trang chủ
        if (controllerName.Equals("Home") && actionName.Equals("Index"))
        {
            urlTemplate.Append(Url.Action("Index", "Home") + "?page={0}");
        }
        else
        {
            // Xây dựng URL bình thường cho các trang khác
            urlTemplate.Append(Url.Action(actionName, controllerName) + "?page={0}");
        }

        var queryParameters = HttpUtility.ParseQueryString(HttpContext.Request.QueryString.ToString());

        foreach (var key in queryParameters.AllKeys)
        {
            // Omit the "page" parameter to avoid repetition (Bỏ qua tham số "page" để tránh lặp lại)
            if (key == "page")
                continue;

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
