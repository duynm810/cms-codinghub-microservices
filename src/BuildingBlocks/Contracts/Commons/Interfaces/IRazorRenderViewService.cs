namespace Contracts.Commons.Interfaces;

public interface IRazorRenderViewService
{
    Task<string> RenderViewAsync<TModel>(string viewName, TModel model);

    Task<string> RenderPartialViewToStringAsync<TModel>(string viewName, TModel model);

    Task<string> RenderViewComponentAsync(string viewComponent, object args);
}