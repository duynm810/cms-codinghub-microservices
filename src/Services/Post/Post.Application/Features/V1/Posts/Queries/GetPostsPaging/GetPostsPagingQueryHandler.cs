using Contracts.Commons.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Domain.Repositories;
using Post.Domain.Services;
using Serilog;
using Shared.Dtos.Post.Queries;
using Shared.Helpers;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsPaging;

public class GetPostsPagingQueryHandler(
    IPostRepository postRepository,
    ICacheService cacheService,
    IPostService postService,
    ILogger logger) : IRequestHandler<GetPostsPagingQuery, ApiResult<PagedResponse<PostDto>>>
{
    public async Task<ApiResult<PagedResponse<PostDto>>> Handle(GetPostsPagingQuery request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<PagedResponse<PostDto>>();
        const string methodName = nameof(GetPostsPagingQuery);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving posts for page {PageNumber} with page size {PageSize}", methodName, request.PageNumber, request.PageSize);

            var cacheKey = CacheKeyHelper.Post.GetPostsPagingKey(request.PageNumber, request.PageSize);
            var cachedPosts = await cacheService.GetAsync<PagedResponse<PostDto>>(cacheKey, cancellationToken);
            if (cachedPosts != null)
            {
                logger.Information("END {MethodName} - Successfully retrieved posts paging from cache", methodName);
                result.Success(cachedPosts);
                return result;
            }

            var posts = await postRepository.GetPostsPaging(request.Filter, request.PageNumber, request.PageSize);
            if (posts.Items != null && posts.Items.IsNotNullOrEmpty())
            {
                var data = await postService.EnrichPagedPostsWithCategories(posts, cancellationToken);

                result.Success(data);

                // Save cache (Lưu cache)
                await cacheService.SetAsync(cacheKey, data, cancellationToken: cancellationToken);

                logger.Information("END {MethodName} - Successfully retrieved {PostCount} posts for page {PageNumber} with page size {PageSize}", methodName, data.MetaData.TotalItems, request.PageNumber, request.PageSize);
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