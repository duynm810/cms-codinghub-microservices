using Microsoft.AspNetCore.Mvc;

namespace Post.API.Controllers;

public class HomeController : ControllerBase
{
    public IActionResult Index()
    {
        return Redirect("~/swagger");
    }
}