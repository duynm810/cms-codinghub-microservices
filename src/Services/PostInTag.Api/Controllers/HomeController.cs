using Microsoft.AspNetCore.Mvc;

namespace PostInTag.Api.Controllers;

public class HomeController : ControllerBase
{
    public IActionResult Index()
    {
        return Redirect("~/swagger");
    }
}