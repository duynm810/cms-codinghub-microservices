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
using Post.Application.Features.V1.Posts.Commands.UpdatePost;
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
using Shared.Dtos.Post.Commands;
using Shared.Dtos.Post.Queries;
using Shared.Extensions;
using Shared.Responses;

namespace Post.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(IdentityServerAuthenticationDefaults.AuthenticationScheme)]
public class PostsController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<long>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ApiResult<long>>> CreatePost([FromBody] CreatePostDto request)
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
    public async Task<ActionResult<PostDto>> UpdatePost([Required] Guid id, [FromBody] UpdatePostCommand command)
    {
        command.SetId(id);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(NoContentResult), (int)HttpStatusCode.NoContent)]
    public async Task<ActionResult> DeletePost([Required] Guid id)
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
    public async Task<ActionResult<PostDto>> GetPostById([Required] Guid id)
    {
        var query = new GetPostByIdQuery(id);
        var result = await mediator.Send(query);
        return Ok(result);
    }
    
    [HttpGet("slug/{slug}")]
    [ProducesResponseType(typeof(ApiResult<PostDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<PostDto>> GetPostBySlug([Required] string slug)
    {
        var query = new GetPostBySlugQuery(slug);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("paging")]
    [ProducesResponseType(typeof(ApiResult<PagedResponse<PostDto>>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public async Task<IActionResult> GetPostsPaging(
        [FromQuery] string? filter,
        [FromQuery, Required] int pageNumber = 1,
        [FromQuery, Required] int pageSize = 10)
    {
        var query = new GetPostsPagingQuery(filter, pageNumber, pageSize);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("by-category/{categorySlug}/paging")]
    [ProducesResponseType(typeof(ApiResult<PagedResponse<PostDto>>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public async Task<IActionResult> GetPostsByCategoryPaging(
        [FromRoute] string categorySlug,
        [FromQuery, Required] int pageNumber = 1,
        [FromQuery, Required] int pageSize = 10)
    {
        var query = new GetPostsByCategoryPagingQuery(categorySlug, pageNumber, pageSize);
        var result = await mediator.Send(query);
        return Ok(result);
    }
    
    [HttpGet("by-tag/{tagSlug}/paging")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPostByTagPaging(string tagSlug, [FromQuery, Required] int pageNumber = 1,
        [FromQuery, Required] int pageSize = 10)
    {
        var query = new GetPostsByTagPagingQuery(tagSlug, pageNumber, pageSize);
        var result = await mediator.Send(query);
        return Ok(result);
    }
    
    [HttpGet("by-series/{seriesSlug}/paging")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPostBySeriesPaging(string seriesSlug, [FromQuery, Required] int pageNumber = 1,
        [FromQuery, Required] int pageSize = 10)
    {
        var query = new GetPostsBySeriesPagingQuery(seriesSlug, pageNumber, pageSize);
        var result = await mediator.Send(query);
        return Ok(result);
    }
    
    [HttpGet("by-author/{username}/paging")]
    [ProducesResponseType(typeof(ApiResult<PagedResponse<PostDto>>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public async Task<IActionResult> GetPostsByAuthorPaging(
        [FromRoute] string username,
        [FromQuery, Required] int pageNumber = 1,
        [FromQuery, Required] int pageSize = 10)
    {
        var query = new GetPostsByAuthorPagingQuery(username, pageNumber, pageSize);
        var result = await mediator.Send(query);
        return Ok(result);
    }
    
    [HttpGet("by-current-user/paging")]
    [ProducesResponseType(typeof(ApiResult<PagedResponse<PostDto>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetPostsByCurrentUserPaging(
        [FromQuery, Required] int pageNumber = 1,
        [FromQuery, Required] int pageSize = 10)
    {
        var userId = User.GetUserId();
        var query = new GetPostsByCurrentUserPagingQuery(userId, pageNumber, pageSize);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("latest/paging")]
    [ProducesResponseType(typeof(ApiResult<PagedResponse<PostDto>>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public async Task<IActionResult> GetLatestPostsPaging(
        [FromQuery, Required] int pageNumber = 1,
        [FromQuery, Required] int pageSize = 10)
    {
        var query = new GetLatestPostsPagingQuery(pageNumber, pageSize);
        var result = await mediator.Send(query);
        return Ok(result);
    }
    
    [HttpGet("detail/by-slug/{slug}")]
    [ProducesResponseType(typeof(ApiResult<PostDto>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public async Task<ActionResult<PostDto>> GetDetailBySlug([Required] string slug, int relatedCount)
    {
        var query = new GetDetailBySlugQuery(slug, relatedCount);
        var result = await mediator.Send(query);
        return Ok(result);
    }
    
    [HttpGet("featured")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<PostDto>>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public async Task<IActionResult> GetFeaturedPosts([FromQuery] int count = 4)
    {
        var query = new GetFeaturedPostsQuery(count);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("pinned")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<PostDto>>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public async Task<IActionResult> GetPinnedPosts([FromQuery] int count = 4)
    {
        var query = new GetPinnedPostsQuery(count);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("most-commented")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<PostDto>>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public async Task<IActionResult> GetMostCommentedPosts([FromQuery] int count = 6)
    {
        var query = new GetMostCommentPostsQuery(count);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("most-liked")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<PostDto>>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public async Task<IActionResult> GetMostLikedPosts([FromQuery] int count = 4)
    {
        var query = new GetMostLikedPostsQuery(count);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("by-non-static-page-category")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<PostsByNonStaticPageCategoryDto>>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public async Task<IActionResult> GetPostsByNonStaticPageCategory([FromQuery] int count = 3)
    {
        var query = new GetPostsByNonStaticPageCategoryQuery(count);
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
    public async Task<IActionResult> SubmitPostForApproval(Guid id)
    {
        var command = new SubmitPostForApprovalCommand(id, User.GetUserId());
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("reject/{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<bool>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> RejectPostWithReasonCommand(Guid id, [FromBody] RejectPostWithReasonDto request)
    {
        var command = new RejectPostWithReasonCommand(id, User.GetUserId(), request);
        var result = await mediator.Send(command);
        return Ok(result);
    }
}