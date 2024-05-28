using Microsoft.AspNetCore.Mvc;

namespace WebApps.UI.Controllers;

public class ContactController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}