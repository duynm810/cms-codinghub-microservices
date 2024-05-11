using Microsoft.AspNetCore.Mvc;

namespace Series.Api.Controllers;

public class HomeController : ControllerBase
{
    public IActionResult Index()
    {
        return Redirect("~/swagger");
    }
}