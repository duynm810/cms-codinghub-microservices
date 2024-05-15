using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Api.Controllers;

public class HomeController : ControllerBase
{
    public IActionResult Index()
    {
        return Redirect("~/jobs");
    }
}