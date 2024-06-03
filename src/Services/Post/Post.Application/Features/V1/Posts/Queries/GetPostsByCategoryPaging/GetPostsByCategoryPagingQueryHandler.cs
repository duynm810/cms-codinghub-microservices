using AutoMapper;
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
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsByCategoryPaging;

public class GetPostsByCategoryPagingQueryHandler(
    IPostRepository postRepository,
    ICategoryGrpcService categoryGrpcService,
    IDistributedCache redisCacheService,
    ISerializeService serializeService,
    IMappingHelper mappingHelper,
    ILogger logger) : IRequestHandler<GetPostsByCategoryPagingQuery, ApiResult<PagedResponse<PostModel>>>
{
    public async Task<ApiResult<PagedResponse<PostModel>>> Handle(GetPostsByCategoryPagingQuery request,
        CancellationToken cancellationToken)
    {
        var result = new ApiResult<PagedResponse<PostModel>>();
        const string methodName = nameof(GetPostsByCategoryPagingQuery);

        try
        {
            logger.Information(
                "BEGIN {MethodName} - Retrieving posts for category slug {CategorySlug} on page {PageNumber} with page size {PageSize}",
                methodName, request.CategorySlug, request.PageNumber, request.PageSize);
            
            var cacheKey = $"posts_by_category_{request.CategorySlug}_page_{request.PageNumber}_size_{request.PageSize}";
            // Kiểm tra cache
            var cachedPosts = await redisCacheService.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrEmpty(cachedPosts))
            {
                var cachedData = serializeService.Deserialize<PagedResponse<PostModel>>(cachedPosts);
                if (cachedData != null)
                {
                    result.Success(cachedData);
                    logger.Information(
                        "END {MethodName} - Successfully retrieved posts from cache for category slug {CategorySlug} on page {PageNumber} with page size {PageSize}",
                        methodName, request.CategorySlug, request.PageNumber, request.PageSize);
                    return result;
                }
            }

            var category = await categoryGrpcService.GetCategoryBySlug(request.CategorySlug);
            if (category == null)
            {
                logger.Warning("{MethodName} - Category not found with slug: {CategorySlug}", methodName,
                    request.CategorySlug);
                result.Messages.Add(ErrorMessagesConsts.Category.CategoryNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var posts = await postRepository.GetPostsByCategoryPaging(category.Id, request.PageNumber,
                request.PageSize);

            var data = mappingHelper.MapPostsWithCategory(posts, category);
            result.Success(data);

            // Lưu cache
            var serializedData = serializeService.Serialize(data);
            await redisCacheService.SetStringAsync(cacheKey, serializedData, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // Cache trong 5 phút
            }, cancellationToken);

            logger.Information(
                "END {MethodName} - Successfully retrieved {PostCount} posts for category slug {CategorySlug} on page {PageNumber} with page size {PageSize}",
                methodName, data.MetaData.TotalItems, request.CategorySlug, request.PageNumber, request.PageSize);
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