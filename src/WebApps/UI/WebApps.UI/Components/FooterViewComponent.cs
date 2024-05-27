using Microsoft.AspNetCore.Mvc;

namespace WebApps.UI.Components;

public class FooterViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        return await Task.FromResult((IViewComponentResult)View("Default"));
    }
}