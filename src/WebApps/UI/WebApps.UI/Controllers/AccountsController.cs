using Contracts.Commons.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shared.Dtos.Category;
using Shared.Requests.Identity.User;
using Shared.Requests.Post.Commands;
using Shared.Requests.Post.Queries;
using Shared.Settings;
using WebApps.UI.ApiClients.Interfaces;
using WebApps.UI.Models.Accounts;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Controllers;

[Authorize]
public class AccountsController(
    IPostApiClient postApiClient,
    ICategoryApiClient categoryApiClient,
    IIdentityApiClient identityApiClient,
    IRazorRenderViewService razorRenderViewService,
    IOptions<ApiSettings> apiSettings,
    ILogger logger) : BaseController(logger)
{
    private readonly ApiSettings _apiSettings = apiSettings.Value;

    #region AUTHENTICATION

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

    #endregion
    
    #region PROFILE

    public async Task<IActionResult> Profile()
    {
        const string methodName = nameof(Profile);

        try
        {
            var response = await identityApiClient.GetMe();
            if (response is not { IsSuccess: true, Data: not null })
            {
                return HandleError(methodName, response.StatusCode);
            }
            
            var items = new ProfileViewModel()
            {
                User = response.Data
            };

            return View(items);
        }
        catch (Exception e)
        {
            return HandleException(methodName, e);
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromRoute] Guid userId, [FromBody] UpdateUserRequest request)
    {
        const string methodName = nameof(UpdateProfile);

        try
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Failed to update profile.";
                return BadRequest(ModelState);
            }

            var updateResponse = await identityApiClient.UpdateUser(userId, request);

            if (updateResponse is not { IsSuccess: true })
            {
                return Json(new { success = false });
            }

            var getUserResponse = await identityApiClient.GetUserById(userId);
            if (getUserResponse is not { IsSuccess: true, Data: not null })
            {
                return Json(new { success = false });
            }

            return Json(new { success = true, user = getUserResponse.Data });
        }
        catch (Exception e)
        {
            return HandleException(methodName, e);
        }
    }

    #endregion

    #region POST
    
    [HttpPost]
    public async Task<IActionResult> GetPostsByCurrentUser([FromBody] GetPostsByCurrentUserRequest request)
    {
        try
        {
            var response = await postApiClient.GetPostsByCurrentUserPaging(request);
            if (response is not { IsSuccess: true, Data: not null })
            {
                return Json(new { success = false });
            }
            
            var items = new ManagePostsViewModel()
            {
                Posts = response.Data
            };

            var html = await razorRenderViewService.RenderPartialViewToStringAsync("~/Views/Shared/Partials/Accounts/_PostsByCurrentUserTablePartial.cshtml", items);
            var paginationHtml = await razorRenderViewService.RenderViewComponentAsync("Pager", new { metaData = items.Posts.MetaData });
            return Json(new { success = true, html, paginationHtml });
        }
        catch (Exception e)
        {
            return HandleException(nameof(GetPostsByCurrentUser), e);
        }
    }

    public async Task<IActionResult> ManagePosts([FromQuery] int page = 1)
    {
        const string methodName = nameof(ManagePosts);
        
        try
        {
            var response = await postApiClient.GetPostsByCurrentUserPaging(new GetPostsByCurrentUserRequest { PageNumber = page });
            if (response is not { IsSuccess: true, Data: not null })
            {
                return HandleError(methodName, response.StatusCode);
            }
            
            var items = new ManagePostsViewModel()
            {
                Posts = response.Data
            };

            return View(items);
        }
        catch (Exception e)
        {
            return HandleException(methodName, e);
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

            ViewData["ServerUrl"] = $"{_apiSettings.ServerUrl}:{_apiSettings.Port}";
            
            return View(items);
        }
        catch (Exception e)
        {
            return HandleException(methodName, e);
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
                var response = await postApiClient.CreatePost(request);
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
            return HandleException(methodName, e);
        }
    }

    [HttpGet]
    public async Task<IActionResult> UpdatePost([FromQuery] string slug)
    {
        const string methodName = nameof(UpdatePost);

        try
        {
            var categories = await GetCategories();

            var response = await postApiClient.GetPostBySlug(slug);
            if (response is not { IsSuccess: true, Data: not null })
            {
                return HandleError(methodName, response.StatusCode);
            }
            
            var items = new UpdatePostViewModel()
            {
                Post = response.Data,
                Categories = categories
            };

            ViewData["ServerUrl"] = $"{_apiSettings.ServerUrl}:{_apiSettings.Port}";
                
            return View(items);
        }
        catch (Exception e)
        {
            return HandleException(methodName, e);
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

            var response = await postApiClient.UpdatePost(id, request);
            
            return response is not { IsSuccess: true }
                ? Json(new { success = false })
                : Json(new { success = true, redirectUrl = Url.Action("ManagePosts", "Accounts") });
        }
        catch (Exception e)
        {
            return HandleException(methodName, e);
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateThumbnail([FromRoute] Guid id, [FromBody] UpdateThumbnailRequest request)
    {
        try
        {
            var response = await postApiClient.UpdateThumbnail(id, request);
            return Ok(new { data = response });
        }
        catch (Exception e)
        {
            return HandleException(nameof(UpdateThumbnail), e);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeletePost([FromRoute] Guid id, [FromQuery] int page = 1)
    {
        try
        {
            var response = await postApiClient.DeletePost(id);
            if (response is not { IsSuccess: true })
            {
                return Json(new { success = false });
            }
            
            return await GetPostsByCurrentUserAndRenderHtml(page);
        }
        catch (Exception e)
        {
            return HandleException(nameof(DeletePost), e);
        }
    }

    [HttpPut]
    public async Task<IActionResult> ApprovePost([FromRoute] Guid id, [FromBody] ApprovePostRequest request)
    {
        try
        {
            var response = await postApiClient.ApprovePost(id, request);
            if (response is not { IsSuccess: true })
            {
                return Json(new { success = false });
            }
            
            return await GetPostsByCurrentUserAndRenderHtml(request.CurrentPage);
        }
        catch (Exception e)
        {
            return HandleException(nameof(ApprovePost), e);
        }
    }
    
    [HttpPut]
    public async Task<IActionResult> SubmitPostForApproval([FromRoute] Guid id, [FromBody] SubmitPostForApprovalRequest request)
    {
        try
        {
            var response = await postApiClient.SubmitPostForApproval(id, request);
            if (response is not { IsSuccess: true })
            {
                return Json(new { success = false } );
            }
            
            return await GetPostsByCurrentUserAndRenderHtml(request.CurrentPage);
        }
        catch (Exception e)
        {
            return HandleException(nameof(SubmitPostForApproval), e);
        }
    }
    
    [HttpPut]
    public async Task<IActionResult> RejectPostWithReason([FromRoute] Guid id, [FromBody] RejectPostWithReasonRequest request)
    {
        try
        {
            var response = await postApiClient.RejectPostWithReason(id, request);
            if (response is not { IsSuccess: true })
            {
                return Json(new { success = false } );
            }
            
            return await GetPostsByCurrentUserAndRenderHtml(request.CurrentPage);
        }
        catch (Exception e)
        {
            return HandleException(nameof(RejectPostWithReason), e);
        }
    }

    [HttpPut]
    public async Task<IActionResult> TogglePinStatus([FromRoute] Guid id, [FromBody] TogglePinStatusRequest request)
    {
        try
        {
            var response = await postApiClient.TogglePinStatus(id, request);
            if (response is not { IsSuccess: true })
            {
                return Json(new { success = false } );
            }
        
            return await GetPostsByCurrentUserAndRenderHtml(request.CurrentPage);
        }
        catch (Exception e)
        {
            return HandleException(nameof(TogglePinStatus), e);
        }
    }
    
    [HttpPut]
    public async Task<IActionResult> ToggleFeaturedStatus([FromRoute] Guid id, [FromBody] ToggleFeaturedStatusRequest request)
    {
        try
        {
            var response = await postApiClient.ToggleFeaturedStatus(id, request);
            if (response is not { IsSuccess: true })
            {
                return Json(new { success = false });
            }

            return await GetPostsByCurrentUserAndRenderHtml(request.CurrentPage);
        }
        catch (Exception e)
        {
            return HandleException(nameof(TogglePinStatus), e);
        }
    }

    #endregion
    
    #region HELPERS

    private async Task<List<CategoryDto>> GetCategories()
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
    
    private async Task<IActionResult> GetPostsByCurrentUserAndRenderHtml(int currentPage)
    {
        var postsResult = await postApiClient.GetPostsByCurrentUserPaging(new GetPostsByCurrentUserRequest { PageNumber = currentPage });
        if (postsResult is not { IsSuccess: true, Data: not null })
        {
            return Json(new { success = false });
        }

        var items = new ManagePostsViewModel
        {
            Posts = postsResult.Data
        };

        var html = await razorRenderViewService.RenderPartialViewToStringAsync("~/Views/Shared/Partials/Accounts/_PostsByCurrentUserTablePartial.cshtml", items);
        var paginationHtml = await razorRenderViewService.RenderViewComponentAsync("Pager", new { metaData = items.Posts.MetaData });
        return Json(new { success = true, html, paginationHtml });
    }

    #endregion
}