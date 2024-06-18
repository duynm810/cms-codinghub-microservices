using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Extensions;
using WebApps.UI.Models.Accounts;

namespace WebApps.UI.Controllers;

[Authorize]
public class AccountsController : Controller
{
    public IActionResult Login(string returnUrl = "/")
    {
        var redirectUrl = Url.Action(nameof(LoginCallback), "Accounts", new { returnUrl });
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
    }

    public async Task<IActionResult> LoginCallback(string returnUrl = "/")
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!authenticateResult.Succeeded)
        {
            return RedirectToAction(nameof(Login));
        }

        return Redirect(returnUrl);
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Profile()
    {
        var item = new ProfileViewModel()
        {
            ShowSiteBottom = false
        };
        
        return View(item);
    }

    public IActionResult ManagePosts()
    {
        var user = User.GetUserId();
        
        var items = new ManagePostsViewModel()
        {
            MainClass = "bg-grey pb-30",
            ShowSiteBottom = false
        };

        return View(items);
    }

    public IActionResult CreatePost()
    {
        var item = new CreatePostViewModel
        {
            MainClass = "bg-grey pb-30",
            ShowSiteBottom = false
        };
        
        return View(item);
    }
}