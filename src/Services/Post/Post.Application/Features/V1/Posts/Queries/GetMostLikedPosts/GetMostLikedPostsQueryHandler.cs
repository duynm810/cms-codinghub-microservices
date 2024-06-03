using Contracts.Commons.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Post.Application.Commons.Mappings.Interfaces;
using Post.Application.Commons.Models;
using Post.Domain.GrpcServices;
using Post.Domain.Repositories;
using Serilog;
using Shared.Constants;
using Shared.Responses;
using Shared.Settings;

namespace Post.Application.Features.V1.Posts.Queries.GetMostLikedPosts;

public class GetMostLikedPostsQueryHandler(
    IPostRepository postRepository,
    ICategoryGrpcService categoryGrpcService,
    IDistributedCache redisCacheService,
    ISerializeService serializeService,
    DisplaySettings displaySettings,
    IMappingHelper mappingHelper,
    ILogger logger) : IRequestHandler<GetMostLikedPostsQuery, ApiResult<IEnumerable<PostModel>>>
{
    public async Task<ApiResult<IEnumerable<PostModel>>> Handle(GetMostLikedPostsQuery request,
        CancellationToken cancellationToken)
    {
        var result = new ApiResult<IEnumerable<PostModel>>();
        const string methodName = nameof(GetMostLikedPostsQuery);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving most liked posts", methodName);
            
            var cacheKey = "most_liked_posts";

            // Kiểm tra cache
            var cachedPosts = await redisCacheService.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrEmpty(cachedPosts))
            {
                var cachedData = serializeService.Deserialize<IEnumerable<PostModel>>(cachedPosts);
                if (cachedData != null)
                {
                    result.Success(cachedData);
                    logger.Information("END {MethodName} - Successfully retrieved most liked posts from cache", methodName);
                    return result;
                }
            }

            var posts = await postRepository.GetMostLikedPosts(
                displaySettings.Config.GetValueOrDefault(DisplaySettingsConsts.Post.MostLikedPosts, 0));

            var postList = posts.ToList();
            
            if (postList.Any())
            {
                var categoryIds = postList.Select(p => p.CategoryId).Distinct().ToList();
                var categories = await categoryGrpcService.GetCategoriesByIds(categoryIds);

                var data = mappingHelper.MapPostsWithCategories(postList, categories);
                result.Success(data);
                
                // Lưu cache
                var serializedData = serializeService.Serialize(data);
                await redisCacheService.SetStringAsync(cacheKey, serializedData, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // Cache trong 5 phút
                }, cancellationToken);

                logger.Information("END {MethodName} - Successfully retrieved {PostCount} most liked posts", methodName, data.Count);
            }
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e.Message);
            result.Messages.Add(e.Message);
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }
}
