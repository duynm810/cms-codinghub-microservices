using Contracts.Commons.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Post.Application.Commons.Models;
using Post.Domain.GrpcServices;
using Post.Domain.Repositories;
using Serilog;
using Shared.Constants;
using Shared.Responses;
using Shared.Settings;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsByNonStaticPageCategory;

public class GetPostsByNonStaticPageCategoryQueryHandler(
    IPostRepository postRepository,
    ICategoryGrpcService categoryGrpcService,
    IDistributedCache redisCacheService,
    ISerializeService serializeService,
    DisplaySettings displaySettings,
    ILogger logger)
    : IRequestHandler<GetPostsByNonStaticPageCategoryQuery, ApiResult<IEnumerable<CategoryWithPostsModel>>>
{
    public async Task<ApiResult<IEnumerable<CategoryWithPostsModel>>> Handle(
        GetPostsByNonStaticPageCategoryQuery request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<IEnumerable<CategoryWithPostsModel>>();
        const string methodName = nameof(GetPostsByNonStaticPageCategoryQuery);
        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving posts by non-static page categories", methodName);
            
            var cacheKey = "posts_by_non_static_page_category";
            
            // Kiểm tra cache
            var cachedData = await redisCacheService.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrEmpty(cachedData))
            {
                var cachedCategoriesWithPosts = serializeService.Deserialize<IEnumerable<CategoryWithPostsModel>>(cachedData);
                if (cachedCategoriesWithPosts != null)
                {
                    result.Success(cachedCategoriesWithPosts);
                    logger.Information("END {MethodName} - Successfully retrieved posts by non-static page categories from cache", methodName);
                    return result;
                }
            }

            var nonStaticPageCategories = await categoryGrpcService.GetAllNonStaticPageCategories();
            
            var data = new List<CategoryWithPostsModel>();
            
            foreach (var category in nonStaticPageCategories)
            {
                if (category == null) 
                    continue;
                
                var posts = await postRepository.GetPostsByCategoryId(category.Id, displaySettings.Config.GetValueOrDefault(DisplaySettingsConsts.Post.PostsByNonStaticPageCategory, 0));

                var postList = posts.ToList();
                
                if (postList.Any())
                {
                    var postSummaries = postList.Select(post => new PostModel
                    {
                        Id = post.Id,
                        Title = post.Title,
                        Slug = post.Slug,
                        Thumbnail = post.Thumbnail,
                        PublishedDate = post.PublishedDate,
                        ViewCount = post.ViewCount
                    }).ToList();

                    var categoryWithPosts = new CategoryWithPostsModel
                    {
                        CategoryId = category.Id,
                        CategoryName = category.Name,
                        CategorySlug = category.Slug,
                        Posts = postSummaries
                    };

                    data.Add(categoryWithPosts);
                }
            }
            
            result.Success(data);
            
            // Lưu cache
            var serializedData = serializeService.Serialize(data);
            await redisCacheService.SetStringAsync(cacheKey, serializedData, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // Cache trong 5 phút
            }, cancellationToken);

            logger.Information("END {MethodName} - Successfully retrieved posts by non-static page categories", methodName);
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