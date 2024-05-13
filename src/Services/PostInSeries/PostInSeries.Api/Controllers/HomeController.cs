using Microsoft.AspNetCore.Mvc;

namespace PostInSeries.Api.Controllers;

public class HomeController : ControllerBase
{
    public IActionResult Index()
    {
        return Redirect("~/swagger");
    }
}