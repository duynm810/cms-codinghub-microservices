using Microsoft.AspNetCore.Mvc;

namespace Media.Api.Controllers;

public class HomeController : ControllerBase
{
    public IActionResult Index()
    {
        return Redirect("~/swagger");
    }
}