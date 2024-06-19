using Contracts.Commons.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Application.Commons.Models;
using Post.Domain.GrpcClients;
using Post.Domain.Repositories;
using Serilog;
using Shared.Constants;
using Shared.Dtos.Category;
using Shared.Helpers;
using Shared.Responses;
using Shared.Settings;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsByNonStaticPageCategory;

public class GetPostsByNonStaticPageCategoryQueryHandler(
    IPostRepository postRepository,
    ICategoryGrpcClient categoryGrpcClient,
    ICacheService cacheService,
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

            // Check existed cache (Kiểm tra cache)
            var cacheKey = CacheKeyHelper.Post.GetPostsByNonStaticPageCategoryKey();
            var cachedPosts = await cacheService.GetAsync<IEnumerable<CategoryWithPostsModel>>(cacheKey, cancellationToken);
            if (cachedPosts != null)
            {
                result.Success(cachedPosts);
                logger.Information(
                    "END {MethodName} - Successfully retrieved posts by non-static page categories from cache",
                    methodName);
                return result;
            }

            var nonStaticPageCategories = await categoryGrpcClient.GetAllNonStaticPageCategories();

            var data = new List<CategoryWithPostsModel>();
            foreach (var category in nonStaticPageCategories)
            {
                var posts = await postRepository.GetPostsByCategoryId(category.Id, request.Count);

                var postList = posts.ToList();

                if (postList.Count != 0)
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

            // Save cache (Lưu cache)
            await cacheService.SetAsync(cacheKey, data, cancellationToken: cancellationToken);

            logger.Information("END {MethodName} - Successfully retrieved posts by non-static page categories", methodName);

            result.Messages.Add(ErrorMessagesConsts.Post.InvalidGetPostsByNonStaticPageCategoryNotFound);
            result.Failure(StatusCodes.Status400BadRequest,result.Messages);
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