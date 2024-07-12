using Contracts.Commons.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Domain.Repositories;
using Post.Domain.Services;
using Serilog;
using Shared.Constants;
using Shared.Dtos.Identity.User;
using Shared.Dtos.Post.Queries;
using Shared.Helpers;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsByCurrentUserPaging;

public class GetPostsByCurrentUserPagingQueryHandler(
    IPostRepository postRepository,
    IPostService postService,
    ILogger logger) : IRequestHandler<GetPostsByCurrentUserPagingQuery, ApiResult<PagedResponse<PostDto>>>
{
    public async Task<ApiResult<PagedResponse<PostDto>>> Handle(GetPostsByCurrentUserPagingQuery request,
        CancellationToken cancellationToken)
    {
        var result = new ApiResult<PagedResponse<PostDto>>();
        const string methodName = nameof(GetPostsByCurrentUserPagingQuery);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving posts for current user {CurrentUserId} on page {PageNumber} with page size {PageSize}", methodName, request.CurrentUser.UserId, request.PageNumber, request.PageSize);

            var posts = await postRepository.GetPostsByCurrentUserPaging(request.Filter, request.CurrentUser, request.PageNumber, request.PageSize);
            if (posts.Items != null && posts.Items.IsNotNullOrEmpty())
            {
                var data = await postService.EnrichPagedPostsWithCategories(posts, cancellationToken);
                
                result.Success(data);

                logger.Information("END {MethodName} - Successfully retrieved {PostCount} posts for current user {CurrentUserId} for page {PageNumber} with page size {PageSize}", methodName, data.MetaData.TotalItems, request.CurrentUser.UserId, request.PageNumber, request.PageSize);
            }
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }
}