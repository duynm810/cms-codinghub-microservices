using Microsoft.AspNetCore.Mvc;

namespace Category.Api.Controllers;

public class HomeController : ControllerBase
{
    public IActionResult Index()
    {
        return Redirect("~/swagger");
    }
}