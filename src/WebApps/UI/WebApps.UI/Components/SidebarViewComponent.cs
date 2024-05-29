using Microsoft.AspNetCore.Mvc;
using WebApps.UI.Models;

namespace WebApps.UI.Components;

public class SidebarViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var items = new SidebarViewModel
        {
        };

        return View(items);
    }
}