using Contracts.Commons.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Post.Application.Commons.Mappings.Interfaces;
using Post.Application.Commons.Models;
using Post.Domain.GrpcServices;
using Post.Domain.Repositories;
using Serilog;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetLatestPostsPaging;

public class GetLatestPostsPagingQueryHandler(
    IPostRepository postRepository,
    ICategoryGrpcService categoryGrpcService,
    IDistributedCache redisCacheService,
    ISerializeService serializeService,
    IMappingHelper mappingHelper,
    ILogger logger) : IRequestHandler<GetLatestPostsPagingQuery, ApiResult<PagedResponse<PostModel>>>
{
    public async Task<ApiResult<PagedResponse<PostModel>>> Handle(GetLatestPostsPagingQuery request,
        CancellationToken cancellationToken)
    {
        var result = new ApiResult<PagedResponse<PostModel>>();
        const string methodName = nameof(GetLatestPostsPagingQuery);

        try
        {
            logger.Information(
                "BEGIN {MethodName} - Retrieving latest posts for page {PageNumber} with page size {PageSize}",
                methodName, request.PageNumber, request.PageSize);
            
            var cacheKey = $"latest_posts_paging_{request.PageNumber}_{request.PageSize}";
            
            // Kiểm tra cache
            var cachedPosts = await redisCacheService.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrEmpty(cachedPosts))
            {
                var cachedData = serializeService.Deserialize<PagedResponse<PostModel>>(cachedPosts);
                if (cachedData != null)
                {
                    result.Success(cachedData);
                    logger.Information("END {MethodName} - Successfully retrieved latest posts from cache for page {PageNumber} with page size {PageSize}", methodName, request.PageNumber, request.PageSize);
                    return result;
                }
            }

            var posts = await postRepository.GetLatestPostsPaging(request.PageNumber, request.PageSize);

            if (posts.Items != null && posts.Items.IsNotNullOrEmpty())
            {
                var categoryIds = posts.Items.Select(p => p.CategoryId).Distinct().ToList();
                var categories = await categoryGrpcService.GetCategoriesByIds(categoryIds);

                var data = mappingHelper.MapPostsWithCategories(posts, categories);
                result.Success(data);
                
                // Lưu cache
                var serializedData = serializeService.Serialize(data);
                await redisCacheService.SetStringAsync(cacheKey, serializedData, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // Cache trong 5 phút
                }, cancellationToken);

                logger.Information(
                    "END {MethodName} - Successfully retrieved {PostCount} latest posts for page {PageNumber} with page size {PageSize}",
                    methodName, data.MetaData.TotalItems, request.PageNumber, request.PageSize);
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