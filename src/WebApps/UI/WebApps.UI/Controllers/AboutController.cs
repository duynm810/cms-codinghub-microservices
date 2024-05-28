using Microsoft.AspNetCore.Mvc;

namespace WebApps.UI.Controllers;

public class AboutController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}