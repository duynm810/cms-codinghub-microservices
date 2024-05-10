using Microsoft.AspNetCore.Mvc;

namespace Post.Api.Controllers;

public class HomeController : ControllerBase
{
    public IActionResult Index()
    {
        return Redirect("~/swagger");
    }
}