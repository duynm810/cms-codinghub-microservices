using System.ComponentModel.DataAnnotations;
using System.Net;
using AutoMapper;
using IdentityServer4.AccessTokenValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Post.Application.Features.V1.Posts.Commands.ApprovePost;
using Post.Application.Features.V1.Posts.Commands.CreatePost;
using Post.Application.Features.V1.Posts.Commands.DeletePost;
using Post.Application.Features.V1.Posts.Commands.RejectPostWithReason;
using Post.Application.Features.V1.Posts.Commands.SubmitPostForApproval;
using Post.Application.Features.V1.Posts.Commands.ToggleFeaturedStatus;
using Post.Application.Features.V1.Posts.Commands.TogglePinStatus;
using Post.Application.Features.V1.Posts.Commands.UpdatePost;
using Post.Application.Features.V1.Posts.Commands.UpdateThumbnail;
using Post.Application.Features.V1.Posts.Queries.GetDetailBySlug;
using Post.Application.Features.V1.Posts.Queries.GetFeaturedPosts;
using Post.Application.Features.V1.Posts.Queries.GetLatestPostsPaging;
using Post.Application.Features.V1.Posts.Queries.GetMostCommentPosts;
using Post.Application.Features.V1.Posts.Queries.GetMostLikedPosts;
using Post.Application.Features.V1.Posts.Queries.GetPinnedPosts;
using Post.Application.Features.V1.Posts.Queries.GetPostById;
using Post.Application.Features.V1.Posts.Queries.GetPostBySlug;
using Post.Application.Features.V1.Posts.Queries.GetPosts;
using Post.Application.Features.V1.Posts.Queries.GetPostsByAuthorPaging;
using Post.Application.Features.V1.Posts.Queries.GetPostsByCategoryPaging;
using Post.Application.Features.V1.Posts.Queries.GetPostsByCurrentUserPaging;
using Post.Application.Features.V1.Posts.Queries.GetPostsByNonStaticPageCategory;
using Post.Application.Features.V1.Posts.Queries.GetPostsBySeriesPaging;
using Post.Application.Features.V1.Posts.Queries.GetPostsByTagPaging;
using Post.Application.Features.V1.Posts.Queries.GetPostsPaging;
using Shared.Dtos.Post;
using Shared.Extensions;
using Shared.Requests.Post.Commands;
using Shared.Requests.Post.Queries;
using Shared.Responses;

namespace Post.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(IdentityServerAuthenticationDefaults.AuthenticationScheme)]
public class PostsController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<Guid>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ApiResult<Guid>>> CreatePost([FromBody, Required] CreatePostRequest request)
    {
        var command = mapper.Map<CreatePostCommand>(request);
        command.AuthorUserId = User.GetUserId();

        var roles = User.GetRoles();
        command.SetStatusBasedOnRoles(roles);

        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<PostDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<PostDto>> UpdatePost([FromRoute, Required] Guid id,
        [FromBody] UpdatePostCommand command)
    {
        command.SetId(id);
        command.AuthorUserId = User.GetUserId();
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("update-thumbnail/{id:guid}")]
    public async Task<ActionResult<bool>> UpdateThumbnail([FromRoute, Required] Guid id,
        [FromBody, Required] UpdateThumbnailRequest request)
    {
        var command = mapper.Map<UpdateThumbnailCommand>(request);
        command.SetId(id);

        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(NoContentResult), (int)HttpStatusCode.NoContent)]
    public async Task<ActionResult> DeletePost([FromRoute, Required] Guid id)
    {
        var command = new DeletePostCommand(id);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<PostDto>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetPosts()
    {
        var query = new GetPostsQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<PostDto>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public async Task<ActionResult<PostDto>> GetPostById([FromRoute, Required] Guid id)
    {
        var query = new GetPostByIdQuery(id);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("slug/{slug}")]
    [ProducesResponseType(typeof(ApiResult<PostDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<PostDto>> GetPostBySlug([FromRoute, Required] string slug)
    {
        var query = new GetPostBySlugQuery(slug);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("paging")]
    [ProducesResponseType(typeof(ApiResult<PagedResponse<PostDto>>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public async Task<IActionResult> GetPostsPaging([FromBody] GetPostsRequest request)
    {
        var query = new GetPostsPagingQuery(request);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("by-category/{categorySlug}/paging")]
    [ProducesResponseType(typeof(ApiResult<PagedResponse<PostDto>>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public async Task<IActionResult> GetPostsByCategoryPaging([FromRoute] string categorySlug, [FromBody] GetPostsByCategoryRequest request)
    {
        var query = new GetPostsByCategoryPagingQuery(categorySlug, request);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("by-tag/{tagSlug}/paging")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPostsByTagPaging([FromRoute, Required] string tagSlug, [FromBody] GetPostsByTagRequest request)
    {
        var query = new GetPostsByTagPagingQuery(tagSlug, request);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("by-series/{seriesSlug}/paging")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPostBySeriesPaging([FromRoute] string seriesSlug, [FromBody] GetPostBySeriesRequest request)
    {
        var query = new GetPostsBySeriesPagingQuery(seriesSlug, request);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("by-author/{username}/paging")]
    [ProducesResponseType(typeof(ApiResult<PagedResponse<PostDto>>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public async Task<IActionResult> GetPostsByAuthorPaging([FromRoute] string username, [FromBody] GetPostsByAuthorRequest request)
    {
        var query = new GetPostsByAuthorPagingQuery(username, request);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("by-current-user/paging")]
    [ProducesResponseType(typeof(ApiResult<PagedResponse<PostDto>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetPostsByCurrentUserPaging([FromBody] GetPostsByCurrentUserRequest request)
    {
        var currentUser = User.GetCurrentUser();
        var query = new GetPostsByCurrentUserPagingQuery(request, currentUser);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("latest/paging")]
    [ProducesResponseType(typeof(ApiResult<PagedResponse<PostDto>>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public async Task<IActionResult> GetLatestPostsPaging([FromBody] GetLatestPostsRequest request)
    {
        var query = new GetLatestPostsPagingQuery(request);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("detail/by-slug/{slug}")]
    [ProducesResponseType(typeof(ApiResult<PostDto>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public async Task<ActionResult<PostDto>> GetDetailBySlug([FromRoute, Required] string slug, int relatedCount)
    {
        var query = new GetDetailBySlugQuery(slug, relatedCount);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("featured")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<PostDto>>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public async Task<IActionResult> GetFeaturedPosts([FromQuery] int? count)
    {
        var query = new GetFeaturedPostsQuery(count ?? 4);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("pinned")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<PostDto>>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public async Task<IActionResult> GetPinnedPosts([FromQuery] int? count)
    {
        var query = new GetPinnedPostsQuery(count ?? 4);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("most-commented")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<PostDto>>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public async Task<IActionResult> GetMostCommentedPosts([FromQuery] int? count)
    {
        var query = new GetMostCommentPostsQuery(count ?? 6);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("most-liked")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<PostDto>>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public async Task<IActionResult> GetMostLikedPosts([FromQuery] int? count)
    {
        var query = new GetMostLikedPostsQuery(count ?? 4);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("by-non-static-page-category")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<PostsByNonStaticPageCategoryDto>>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public async Task<IActionResult> GetPostsByNonStaticPageCategory([FromQuery] int? count)
    {
        var query = new GetPostsByNonStaticPageCategoryQuery(count ?? 3);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("approve/{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<bool>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ApprovePost(Guid id)
    {
        var command = new ApprovePostCommand(id, User.GetUserId());
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("submit-for-approval/{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<bool>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> SubmitPostForApproval([FromRoute] Guid id)
    {
        var command = new SubmitPostForApprovalCommand(id, User.GetUserId());
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("reject/{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<bool>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> RejectPostWithReasonCommand([FromRoute] Guid id,
        [FromBody] RejectPostWithReasonRequest request)
    {
        var command = new RejectPostWithReasonCommand(id, User.GetUserId(), request);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("toggle-pin-status/{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<bool>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> TogglePinStatusCommand([FromRoute] Guid id, [FromBody] TogglePinStatusRequest request)
    {
        var command = new TogglePinStatusCommand(id, request);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("toggle-featured-status/{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<bool>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ToggleFeaturedStatus([FromRoute] Guid id, [FromBody] ToggleFeaturedStatusRequest request)
    {
        var command = new ToggleFeaturedStatusCommand(id, request);
        var result = await mediator.Send(command);
        return Ok(result);
    }
}