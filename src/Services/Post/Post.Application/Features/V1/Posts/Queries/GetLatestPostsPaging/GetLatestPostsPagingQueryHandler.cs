using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Domain.Repositories;
using Post.Domain.Services;
using Serilog;
using Shared.Dtos.Post.Queries;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetLatestPostsPaging;

public class GetLatestPostsPagingQueryHandler(
    IPostRepository postRepository,
    IPostService postService,
    ILogger logger) : IRequestHandler<GetLatestPostsPagingQuery, ApiResult<PagedResponse<PostDto>>>
{
    public async Task<ApiResult<PagedResponse<PostDto>>> Handle(GetLatestPostsPagingQuery query,
        CancellationToken cancellationToken)
    {
        var result = new ApiResult<PagedResponse<PostDto>>();
        const string methodName = nameof(GetLatestPostsPagingQuery);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving latest posts for page {PageNumber} with page size {PageSize}", methodName, query.Request.PageNumber, query.Request.PageSize);

            var posts = await postRepository.GetLatestPostsPaging(query.Request);

            if (posts.Items != null && posts.Items.IsNotNullOrEmpty())
            {
                var data = await postService.EnrichPagedPostsWithCategories(posts, cancellationToken);
                
                result.Success(data);
                
                logger.Information("END {MethodName} - Successfully retrieved {PostCount} latest posts for page {PageNumber} with page size {PageSize}", methodName, data.MetaData.TotalItems, query.Request.PageNumber, query.Request.PageSize);
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