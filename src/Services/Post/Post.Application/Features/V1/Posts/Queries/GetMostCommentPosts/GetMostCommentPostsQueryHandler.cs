using Contracts.Commons.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Domain.Repositories;
using Post.Domain.Services;
using Serilog;
using Shared.Dtos.Post.Queries;
using Shared.Helpers;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetMostCommentPosts;

public class GetMostCommentPostsQueryHandler(
    IPostRepository postRepository,
    ICacheService cacheService,
    IPostService postService,
    ILogger logger) : IRequestHandler<GetMostCommentPostsQuery, ApiResult<IEnumerable<PostDto>>>
{
    public async Task<ApiResult<IEnumerable<PostDto>>> Handle(GetMostCommentPostsQuery request,
        CancellationToken cancellationToken)
    {
        var result = new ApiResult<IEnumerable<PostDto>>();
        const string methodName = nameof(GetMostCommentPostsQuery);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving most commented posts", methodName);

            var cacheKey = CacheKeyHelper.Post.GetMostCommentPostsKey();
            var cachedPosts = await cacheService.GetAsync<IEnumerable<PostDto>>(cacheKey, cancellationToken);
            if (cachedPosts != null)
            {
                logger.Information("END {MethodName} - Successfully retrieved most commented posts from cache", methodName);
                result.Success(cachedPosts);
                return result;
            }

            var posts = await postRepository.GetMostCommentPosts(request.Count);
            
            var postList = posts.ToList();
            if (postList.Count != 0)
            {
                var data = await postService.EnrichPostsWithCategories(postList, cancellationToken);
                
                result.Success(data);

                // Save cache (Lưu cache)
                await cacheService.SetAsync(cacheKey, data, cancellationToken: cancellationToken);

                logger.Information("END {MethodName} - Successfully retrieved {PostCount} most commented posts", methodName, data.Count);
            }
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.Add(e.Message);
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }
}