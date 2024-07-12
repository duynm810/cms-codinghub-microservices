using Contracts.Commons.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Domain.Repositories;
using Post.Domain.Services;
using Serilog;
using Shared.Dtos.Post;
using Shared.Helpers;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetPinnedPosts;

public class GetPinnedPostsQueryHandler(
    IPostRepository postRepository,
    ICacheService cacheService,
    IPostService postService,
    ILogger logger) : IRequestHandler<GetPinnedPostsQuery, ApiResult<IEnumerable<PostDto>>>
{
    public async Task<ApiResult<IEnumerable<PostDto>>> Handle(GetPinnedPostsQuery query,
        CancellationToken cancellationToken)
    {
        var result = new ApiResult<IEnumerable<PostDto>>();
        const string methodName = nameof(GetPinnedPostsQuery);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving pinned posts", methodName);

            var cacheKey = CacheKeyHelper.Post.GetPinnedPostsKey();
            var cachedPosts = await cacheService.GetAsync<IEnumerable<PostDto>>(cacheKey, cancellationToken);
            if (cachedPosts != null)
            {
                logger.Information("END {MethodName} - Successfully retrieved pinned posts from cache", methodName);
                result.Success(cachedPosts);
                return result;
            }

            var posts = await postRepository.GetPinnedPosts(query.Count);

            var postList = posts.ToList();

            if (postList.IsNotNullOrEmpty())
            {
                var data = await postService.EnrichPostsWithCategories(postList, cancellationToken);
                
                result.Success(data);

                // Save cache (Lưu cache)
                await cacheService.SetAsync(cacheKey, data, cancellationToken: cancellationToken);

                logger.Information("END {MethodName} - Successfully retrieved {PostCount} pinned posts", methodName,
                    data.Count);
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