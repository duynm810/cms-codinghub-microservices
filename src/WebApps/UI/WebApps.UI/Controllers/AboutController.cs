using Microsoft.AspNetCore.Mvc;
using WebApps.UI.Models.About;

namespace WebApps.UI.Controllers;

public class AboutController : Controller
{
    public IActionResult Index()
    {
        var viewModel = new AboutViewModel()
        {
            MainClass = "bg-grey pb-30"
        };

        return View(viewModel);
    }
}