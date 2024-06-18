using Contracts.Commons.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Application.Commons.Mappings.Interfaces;
using Post.Application.Commons.Models;
using Post.Domain.GrpcClients;
using Post.Domain.Repositories;
using Serilog;
using Shared.Helpers;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsByCurrentUserPaging;

public class GetPostsByCurrentUserPagingQueryHandler(
    IPostRepository postRepository,
    ICategoryGrpcClient categoryGrpcClient,
    ICacheService cacheService,
    IMappingHelper mappingHelper,
    ILogger logger) : IRequestHandler<GetPostsByCurrentUserPagingQuery, ApiResult<PagedResponse<PostModel>>>
{
    public async Task<ApiResult<PagedResponse<PostModel>>> Handle(GetPostsByCurrentUserPagingQuery request,
        CancellationToken cancellationToken)
    {
        var result = new ApiResult<PagedResponse<PostModel>>();
        const string methodName = nameof(GetPostsByCurrentUserPagingQuery);

        try
        {
            logger.Information(
                "BEGIN {MethodName} - Retrieving posts for current user {CurrentUserId} on page {PageNumber} with page size {PageSize}",
                methodName, request.CurrentUserId, request.PageNumber, request.PageSize);

            // Check existed cache (Kiểm tra cache)
            var cacheKey = CacheKeyHelper.Post.GetPostsByCurrentUserPagingKey(request.CurrentUserId, request.PageNumber, request.PageSize);
            var cachedPosts = await cacheService.GetAsync<PagedResponse<PostModel>>(cacheKey, cancellationToken);
            if (cachedPosts != null)
            {
                result.Success(cachedPosts);
                logger.Information(
                    "END {MethodName} - Successfully retrieved posts from cache for current user {CurrentUserId} on page {PageNumber} with page size {PageSize}",
                    methodName, request.CurrentUserId, request.PageNumber, request.PageSize);
                return result;
            }
            
            var posts = await postRepository.GetPostsByCurrentUserPaging(request.CurrentUserId, request.PageNumber,
                request.PageSize);

            if (posts.Items != null && posts.Items.IsNotNullOrEmpty())
            {
                var categoryIds = posts.Items.Select(p => p.CategoryId).Distinct().ToList();
                var categories = await categoryGrpcClient.GetCategoriesByIds(categoryIds);

                var data = mappingHelper.MapPostsWithCategories(posts, categories);
                result.Success(data);

                // Save cache (Lưu cache)
                await cacheService.SetAsync(cacheKey, data, cancellationToken: cancellationToken);

                logger.Information(
                    "END {MethodName} - Successfully retrieved {PostCount} posts for current user {CurrentUserId} for page {PageNumber} with page size {PageSize}",
                    methodName, data.MetaData.TotalItems, request.CurrentUserId, request.PageNumber, request.PageSize);
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