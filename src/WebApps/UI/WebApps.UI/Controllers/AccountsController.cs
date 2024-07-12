using System.Net;
using Contracts.Commons.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.Category;
using Shared.Requests.Post.Commands;
using Shared.Requests.Post.Queries;
using WebApps.UI.ApiClients.Interfaces;
using WebApps.UI.Models.Accounts;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Controllers;

[Authorize]
public class AccountsController(
    IPostApiClient postApiClient,
    ICategoryApiClient categoryApiClient,
    IRazorRenderViewService razorRenderViewService,
    IErrorService errorService,
    ILogger logger) : BaseController(errorService, logger)
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

    #region PROFILE

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

    #endregion

    #region POST
    
    public async Task<IActionResult> GetPostsByCurrentUser([FromQuery] int page = 1)
    {
        const string methodName = nameof(GetPostsByCurrentUser);

        try
        {
            var request = new GetPostsByCurrentUserRequest { PageNumber = page };
            var response =await postApiClient.GetPostsByCurrentUserPaging(request);
            if (response is { IsSuccess: true, Data: not null })
            {
                var items = new ManagePostsViewModel()
                {
                    Posts = response.Data
                };

                return PartialView("Partials/Accounts/_PostsByCurrentUserTablePartial", items);
            }

            return HandleError((HttpStatusCode)response.StatusCode, methodName);
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
            var response =await postApiClient.GetPostsByCurrentUserPaging(new GetPostsByCurrentUserRequest { PageNumber = page });
            if (response is { IsSuccess: true, Data: not null })
            {
                var items = new ManagePostsViewModel()
                {
                    Posts = response.Data
                };

                return View(items);
            }

            return HandleError((HttpStatusCode)response.StatusCode, methodName);
        }
        catch (Exception e)
        {
            return HandleException(e, methodName);
        }
    }

    [HttpGet]
    public async Task<IActionResult> CreatePost()
    {
        const string methodName = nameof(CreatePost);

        try
        {
            var categories = await GetCategories();
            var items = new CreatePostViewModel
            {
                Categories = categories
            };

            return View(items);
        }
        catch (Exception e)
        {
            return HandleException(e, methodName);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePost(CreatePostRequest request)
    {
        const string methodName = nameof(CreatePost);

        try
        {
            var categories = await GetCategories();

            if (ModelState.IsValid)
            {
                var response =await postApiClient.CreatePost(request);
                if (response is { IsSuccess: true })
                {
                    return RedirectToAction("ManagePosts", "Accounts");
                }

                TempData["ErrorMessage"] = "Failed to create post.";
            }

            var items = new CreatePostViewModel
            {
                Categories = categories,
                Post = request
            };

            return View(items);
        }
        catch (Exception e)
        {
            return HandleException(e, methodName);
        }
    }

    [HttpGet]
    public async Task<IActionResult> UpdatePost([FromQuery] string slug)
    {
        const string methodName = nameof(UpdatePost);

        try
        {
            var categories = await GetCategories();

            var post = await postApiClient.GetPostBySlug(slug);
            if (post is { IsSuccess: true, Data: not null })
            {
                var items = new UpdatePostViewModel()
                {
                    Post = post.Data,
                    Categories = categories
                };

                return View(items);
            }

            return HandleError((HttpStatusCode)post.StatusCode, methodName);
        }
        catch (Exception e)
        {
            return HandleException(e, methodName);
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdatePost([FromRoute] Guid id, [FromBody] UpdatePostRequest request)
    {
        const string methodName = nameof(UpdatePost);

        try
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Failed to update post.";
                return BadRequest(ModelState);
            }

            var response =await postApiClient.UpdatePost(id, request);
            if (response is { IsSuccess: true })
            {
                return Json(new { success = true, redirectUrl = Url.Action("ManagePosts", "Accounts") });
            }

            return HandleError((HttpStatusCode)response.StatusCode, methodName);
        }
        catch (Exception e)
        {
            return HandleException(e, methodName);
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateThumbnail([FromRoute] Guid id, [FromBody] UpdateThumbnailRequest request)
    {
        var response =await postApiClient.UpdateThumbnail(id, request);
        return Ok(new { data = response });
    }

    [HttpDelete]
    public async Task<IActionResult> DeletePost([FromRoute] Guid id, [FromQuery] int page = 1)
    {
        const string methodName = nameof(DeletePost);
        
        try
        {
            var response =await postApiClient.DeletePost(id);
            if (response is { IsSuccess: true })
            {
                // Lấy lại danh sách bài viết sau khi xóa
                var request = new GetPostsByCurrentUserRequest { PageNumber = page };
                var postsResult = await postApiClient.GetPostsByCurrentUserPaging(request);
                if (postsResult is { IsSuccess: true, Data: not null })
                {
                    var items = new ManagePostsViewModel()
                    {
                        Posts = postsResult.Data
                    };
                    
                    var html = await razorRenderViewService.RenderPartialViewToStringAsync("~/Views/Shared/Partials/Accounts/_PostsByCurrentUserTablePartial.cshtml", items);
                    return Json(new { success = true, html });
                }
                
                return Json(new { success = true, html = string.Empty });
            }

            return HandleError((HttpStatusCode)response.StatusCode, methodName);
        }
        catch (Exception e)
        {
            return HandleException(e, methodName);
        }
    }

    [HttpPut]
    public async Task<IActionResult> TogglePinStatus([FromRoute] Guid id, [FromBody] TogglePinStatusRequest request)
    {
        var response =await postApiClient.TogglePinStatus(id, request);
        if (response is { IsSuccess: true })
        {
            var postsResult = await postApiClient.GetPostsByCurrentUserPaging(new GetPostsByCurrentUserRequest { PageNumber = request.CurrentPage });
            if (postsResult is { IsSuccess: true, Data: not null })
            {
                var items = new ManagePostsViewModel()
                {
                    Posts = postsResult.Data,
                };
            
                var html = await razorRenderViewService.RenderPartialViewToStringAsync("~/Views/Shared/Partials/Accounts/_PostsByCurrentUserTablePartial.cshtml", items);
                return Json(new { success = true, html });
            }
        
            return Json(new { success = true, html = string.Empty });
        }

        return Json(new { success = false });
    }
    
    [HttpPut]
    public async Task<IActionResult> ToggleFeaturedStatus([FromRoute] Guid id, [FromBody] ToggleFeaturedStatusRequest request)
    {
        var response =await postApiClient.ToggleFeaturedStatus(id, request);
        if (response is { IsSuccess: true })
        {
            var postsResult = await postApiClient.GetPostsByCurrentUserPaging(new GetPostsByCurrentUserRequest { PageNumber = request.CurrentPage });
            if (postsResult is { IsSuccess: true, Data: not null })
            {
                var items = new ManagePostsViewModel()
                {
                    Posts = postsResult.Data
                };
            
                var html = await razorRenderViewService.RenderPartialViewToStringAsync("~/Views/Shared/Partials/Accounts/_PostsByCurrentUserTablePartial.cshtml", items);
                return Json(new { success = true, html });
            }
        
            return Json(new { success = true, html = string.Empty });
        }

        return Json(new { success = false });
    }

    #endregion
    
    #region HELPERS

    private async Task<IEnumerable<CategoryDto>> GetCategories()
    {
        const string methodName = nameof(GetCategories);

        try
        {
            var categoriesResponse = await categoryApiClient.GetCategories();
            if (categoriesResponse is { IsSuccess: true, Data: not null })
            {
                return categoriesResponse.Data;
            }

            throw new Exception($"Error fetching categories: {categoriesResponse.StatusCode}");
        }
        catch (Exception e)
        {
            throw new Exception($"Exception in {methodName}: {e.Message}", e);
        }
    }

    #endregion
}