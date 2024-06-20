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
public class AccountsController(IPostApiClient postApiClient, ICategoryApiClient categoryApiClient, IErrorService errorService, ILogger logger) : BaseController(errorService, logger)
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
        const string methodName = nameof(Profile);
        
        try
        {
            var item = new ProfileViewModel()
            {
            };

            return View(item);
        }
        catch (Exception e)
        {
            return HandleException(e, methodName);
        }
    }

    public async Task<IActionResult> ManagePosts([FromQuery] int page = 1)
    {
        const string methodName = nameof(ManagePosts);
        
        try
        {
            var result = await postApiClient.GetPostsByCurrentUserPaging(page, 4);
            if (result is { IsSuccess: true, Data: not null })
            {
                var items = new ManagePostsViewModel()
                {
                    Posts = result.Data
                };

                return View(items);
            }

            return HandleError((HttpStatusCode)result.StatusCode, methodName);
        }
        catch (Exception e)
        {
            return HandleException(e, methodName);
        }
    }

    public async Task<IActionResult> CreatePost()
    {
        const string methodName = nameof(CreatePost);
        
        try
        {
            var categories = await categoryApiClient.GetCategories();
            if (categories is { IsSuccess: true, Data: not null })
            {
                var item = new CreatePostViewModel
                {
                    Categories = categories.Data
                };

                return View(item);
            }
            
            return HandleError((HttpStatusCode)categories.StatusCode, methodName);
        }
        catch (Exception e)
        {
            return HandleException(e, methodName);
        }
    }

    [HttpGet]
    public async Task<IActionResult> UpdatePost(string slug)
    {
        const string methodName = nameof(UpdatePost);
        
        try
        {
            var post = await postApiClient.GetPostBySlug(slug);
            if (post is { IsSuccess: true, Data: not null })
            {
                var categories = await categoryApiClient.GetCategories();
                if (categories is { IsSuccess: true, Data: not null })
                {
                    var items = new UpdatePostViewModel()
                    {
                        Post = post.Data,
                        Categories = categories.Data
                    };

                    return View(items);
                }
                
                return HandleError((HttpStatusCode)categories.StatusCode, methodName);
            }

            return HandleError((HttpStatusCode)post.StatusCode, methodName);
        }
        catch (Exception e)
        {
            return HandleException(e, methodName);
        }
    }
}