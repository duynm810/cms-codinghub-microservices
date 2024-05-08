using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApps.HealthCheck.Models;

namespace WebApps.HealthCheck.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return Redirect("/healthchecks-ui");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}