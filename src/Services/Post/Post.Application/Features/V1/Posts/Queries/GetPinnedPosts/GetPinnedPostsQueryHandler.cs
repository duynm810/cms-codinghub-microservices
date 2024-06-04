using Contracts.Commons.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Application.Commons.Mappings.Interfaces;
using Post.Application.Commons.Models;
using Post.Domain.GrpcServices;
using Post.Domain.Repositories;
using Serilog;
using Shared.Constants;
using Shared.Helpers;
using Shared.Responses;
using Shared.Settings;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetPinnedPosts;

public class GetPinnedPostsQueryHandler(
    IPostRepository postRepository,
    ICategoryGrpcService categoryGrpcService,
    ICacheService cacheService,
    DisplaySettings displaySettings,
    IMappingHelper mappingHelper,
    ILogger logger) : IRequestHandler<GetPinnedPostsQuery, ApiResult<IEnumerable<PostModel>>>
{
    public async Task<ApiResult<IEnumerable<PostModel>>> Handle(GetPinnedPostsQuery request,
        CancellationToken cancellationToken)
    {
        var result = new ApiResult<IEnumerable<PostModel>>();
        const string methodName = nameof(GetPinnedPostsQuery);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving pinned posts", methodName);

            // Kiểm tra cache
            var cacheKey = CacheKeyHelper.Post.GetPinnedPostsKey();
            var cachedPosts = await cacheService.GetAsync<IEnumerable<PostModel>>(cacheKey, cancellationToken);
            if (cachedPosts != null)
            {
                result.Success(cachedPosts);
                logger.Information("END {MethodName} - Successfully retrieved pinned posts from cache", methodName);
                return result;
            }

            var posts = await postRepository.GetPinnedPosts(
                displaySettings.Config.GetValueOrDefault(DisplaySettingsConsts.Post.PinnedPosts, 0));

            var postList = posts.ToList();

            if (postList.IsNotNullOrEmpty())
            {
                var categoryIds = postList.Select(p => p.CategoryId).Distinct().ToList();
                var categories = await categoryGrpcService.GetCategoriesByIds(categoryIds);

                var data = mappingHelper.MapPostsWithCategories(postList, categories);
                result.Success(data);

                // Lưu cache
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