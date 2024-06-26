using Contracts.Commons.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace Infrastructure.Commons;

public class RazorRenderViewService(
    IRazorViewEngine razorView,
    ITempDataProvider tempDataProvider,
    IServiceProvider serviceProvider)
    : IRazorRenderViewService
{
    private readonly EmptyModelMetadataProvider _metadataProvider = new();
    private readonly ModelStateDictionary _modelState = new();

    public async Task<string> RenderViewAsync<TModel>(string viewName, TModel model)
    {
        return await RenderAsync(viewName, model, true);
    }

    public async Task<string> RenderPartialViewToStringAsync<TModel>(string viewName, TModel model)
    {
        return await RenderAsync(viewName, model, false);
    }

    #region Helpers
    
    /// <summary>
    /// Render a view or partial view to a string.
    /// </summary>
    /// <typeparam name="TModel">Data type of the model.</typeparam>
    /// <param name="viewName">Name of view to render.</param>
    /// <param name="model">Model data will be passed to the view.</param>
    /// <param name="isMainPage">Determines whether the view is the main page or not.</param>
    /// <returns>String of view's rendering results.</returns>
    private async Task<string> RenderAsync<TModel>(string viewName, TModel model, bool isMainPage)
    {
        // Get the current action context (Lấy ngữ cảnh hành động hiện tại)
        var actionContext = GetActionContext();
        
        // Find views based on view name and action context (Tìm view dựa trên tên view và ngữ cảnh hành động)
        var view = ResearchView(actionContext, viewName, isMainPage);

        // Use StringWriter to hold render results (Sử dụng StringWriter để giữ kết quả render)
        await using var output = new StringWriter();

        // Create ViewContext with necessary parameters (Tạo ViewContext với các tham số cần thiết)
        var viewContext = new ViewContext(
            actionContext,
            view,
            new ViewDataDictionary<TModel>(_metadataProvider, _modelState)
            {
                Model = model  // Set data to Model (Gán dữ liệu vào ViewDataDictionary)
            },
            new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
            output,
            new HtmlHelperOptions());

        await view.RenderAsync(viewContext);

        return output.ToString();
    }

    /// <summary>
    /// Finds and returns an IView object based on the view name and action context.
    /// </summary>
    /// <param name="actionContext">Current action context.</param>
    /// <param name="viewName">Name of view to search for.</param>
    /// <param name="isMainPage">Determines whether the view is the main page or not.</param>
    /// <returns>IView object if found, otherwise an exception will be thrown.</returns>
    /// <exception cref="InvalidOperationException">Throws an exception if the view cannot be found.</exception>
    private IView ResearchView(ActionContext actionContext, string viewName, bool isMainPage)
    {
        // Try to get view based on view path (Thử lấy view dựa trên đường dẫn view)
        var getViewResult = razorView.GetView(executingFilePath: null, viewPath: viewName, isMainPage: isMainPage);
        if (getViewResult.Success)
        {
            return getViewResult.View;
        }

        // Try to find views based on view name and action context (Thử tìm view dựa trên tên view và ngữ cảnh hành động)
        var findViewResult = razorView.FindView(actionContext, viewName, isMainPage: isMainPage);
        if (findViewResult.Success)
        {
            return findViewResult.View;
        }

        // Combines searched locations from both getViewResult and findViewResult results (Kết hợp các vị trí đã tìm kiếm từ cả hai kết quả getViewResult và findViewResult)
        var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
        
        // Generate detailed error messages with searched locations (Tạo thông báo lỗi chi tiết với các vị trí đã tìm kiếm)
        var errorMessage = string.Join(
            Environment.NewLine,
            new[] { $"Unable to find view '{viewName}'. The following locations were searched:" }.Concat(searchedLocations));

        throw new InvalidOperationException(errorMessage);
    }

    /// <summary>
    /// Create a new ActionContext object with the default HttpContext and RouteData.
    /// </summary>
    /// <returns>ActionContext is initialized with HttpContext, RouteData, and ActionDescriptor.</returns>
    private ActionContext GetActionContext()
    {
        // Create an HttpContext object with RequestServices (Tạo một đối tượng HttpContext với RequestServices)
        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider
        };

        // Get routing data from HttpContext (Lấy dữ liệu định tuyến từ HttpContext)
        var routeData = httpContext.GetRouteData();
        
        // If there are no Routers in RouteData, add a new RouteCollection to Routers (Nếu không có Router nào trong RouteData, thêm một RouteCollection mới vào Routers)
        if (!routeData.Routers.Any())
        {
            // Make sure there is at least one Router to avoid errors when processing requests (Đảm bảo có ít nhất một Router để tránh lỗi khi xử lý yêu cầu)
            routeData.Routers.Add(new RouteCollection());
        }

        return new ActionContext(httpContext, routeData, new ActionDescriptor());
    }

    #endregion
}
