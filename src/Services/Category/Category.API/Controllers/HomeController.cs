using Microsoft.AspNetCore.Mvc;

namespace Category.API.Controllers;

public class HomeController : ControllerBase
{
    public IActionResult Index()
    {
        return Redirect("~/swagger");
    }
}   