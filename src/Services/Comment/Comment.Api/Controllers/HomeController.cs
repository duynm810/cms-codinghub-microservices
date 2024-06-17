using Microsoft.AspNetCore.Mvc;

namespace Comment.Api.Controllers;

public class HomeController : ControllerBase
{
    public IActionResult Index()
    {
        return Redirect("~/swagger");
    }
}