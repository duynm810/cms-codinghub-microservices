using AutoMapper;
using Contracts.Commons.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Post.Application.Commons.Mappings.Interfaces;
using Post.Application.Commons.Models;
using Post.Domain.Entities;
using Post.Domain.GrpcServices;
using Post.Domain.Repositories;
using Serilog;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetPosts;

public class GetPostsQueryHandler(
    IPostRepository postRepository,
    ICategoryGrpcService categoryGrpcService,
    IDistributedCache redisCacheService,
    ISerializeService serializeService,
    IMappingHelper mappingHelper,
    ILogger logger)
    : IRequestHandler<GetPostsQuery, ApiResult<IEnumerable<PostModel>>>
{
    public async Task<ApiResult<IEnumerable<PostModel>>> Handle(GetPostsQuery request,
        CancellationToken cancellationToken)
    {
        var result = new ApiResult<IEnumerable<PostModel>>();
        const string methodName = nameof(GetPostsQuery);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving all posts", methodName);
            
            var cacheKey = "all_posts";
            
            // Kiểm tra cache
            var cachedPosts = await redisCacheService.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrEmpty(cachedPosts))
            {
                var cachedData = serializeService.Deserialize<IEnumerable<PostModel>>(cachedPosts);
                if (cachedData != null)
                {
                    result.Success(cachedData);
                    logger.Information("END {MethodName} - Successfully retrieved all posts from cache", methodName);
                    return result;
                }
            }

            var posts = await postRepository.GetPosts();

            var postList = posts.ToList();
            if (postList.IsNotNullOrEmpty())
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

                logger.Information("END {MethodName} - Successfully retrieved {PostCount} posts", methodName,
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