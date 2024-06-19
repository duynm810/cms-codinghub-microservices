using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApps.UI.ApiServices.Interfaces;
using WebApps.UI.Models.Accounts;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Controllers;

[Authorize]
public class AccountsController(IPostApiClient postApiClient, IErrorService errorService, ILogger logger) : BaseController(errorService, logger)
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
        };
        
        return View(item);
    }

    public async Task<IActionResult> ManagePosts([FromQuery] int page = 1)
    {
        const string methodName = nameof(ManagePosts);
        
        try
        {
            
            var posts = await postApiClient.GetPostsByCurrentUserPaging(page, 4);

            if (posts is { IsSuccess: true, Data: not null })
            {
                var items = new ManagePostsViewModel()
                {
                    Posts = posts.Data
                };

                return View(items);
            }

            return HandleError((HttpStatusCode)posts.StatusCode, methodName);
        }
        catch (Exception e)
        {
            return HandleException(e, methodName);
        }
    }

    public IActionResult CreatePost()
    {
        var item = new CreatePostViewModel
        {
        };
        
        return View(item);
    }
}