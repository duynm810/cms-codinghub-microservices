using Contracts.Commons.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Application.Commons.Mappings.Interfaces;
using Post.Application.Commons.Models;
using Post.Domain.GrpcServices;
using Post.Domain.Repositories;
using Serilog;
using Shared.Helpers;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsPaging;

public class GetPostsPagingQueryHandler(
    IPostRepository postRepository,
    ICategoryGrpcService categoryGrpcService,
    ICacheService cacheService,
    IMappingHelper mappingHelper,
    ILogger logger) : IRequestHandler<GetPostsPagingQuery, ApiResult<PagedResponse<PostModel>>>
{
    public async Task<ApiResult<PagedResponse<PostModel>>> Handle(GetPostsPagingQuery request,
        CancellationToken cancellationToken)
    {
        var result = new ApiResult<PagedResponse<PostModel>>();
        const string methodName = nameof(GetPostsPagingQuery);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving posts for page {PageNumber} with page size {PageSize}",
                methodName, request.PageNumber, request.PageSize);

            // Check existed cache (Kiểm tra cache)
            var cacheKey = CacheKeyHelper.Post.GetPostsPagingKey(request.PageNumber, request.PageSize);
            var cachedPosts = await cacheService.GetAsync<PagedResponse<PostModel>>(cacheKey, cancellationToken);
            if (cachedPosts != null)
            {
                result.Success(cachedPosts);
                logger.Information("END {MethodName} - Successfully retrieved posts paging from cache", methodName);
                return result;
            }

            var posts = await postRepository.GetPostsPaging(request.Filter, request.PageNumber, request.PageSize);

            if (posts.Items != null && posts.Items.IsNotNullOrEmpty())
            {
                var categoryIds = posts.Items.Select(p => p.CategoryId).Distinct().ToList();
                var categories = await categoryGrpcService.GetCategoriesByIds(categoryIds);

                var data = mappingHelper.MapPostsWithCategories(posts, categories);

                result.Success(data);

                // Save cache (Lưu cache)
                await cacheService.SetAsync(cacheKey, data, cancellationToken: cancellationToken);

                logger.Information(
                    "END {MethodName} - Successfully retrieved {PostCount} posts for page {PageNumber} with page size {PageSize}",
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