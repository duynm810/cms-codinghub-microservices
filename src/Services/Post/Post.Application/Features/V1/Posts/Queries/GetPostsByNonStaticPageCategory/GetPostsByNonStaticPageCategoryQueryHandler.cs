using Contracts.Commons.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Domain.GrpcClients;
using Post.Domain.Repositories;
using Serilog;
using Shared.Dtos.Post;
using Shared.Helpers;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsByNonStaticPageCategory;

public class GetPostsByNonStaticPageCategoryQueryHandler(
    IPostRepository postRepository,
    ICategoryGrpcClient categoryGrpcClient,
    ICacheService cacheService,
    ILogger logger)
    : IRequestHandler<GetPostsByNonStaticPageCategoryQuery, ApiResult<IEnumerable<PostsByNonStaticPageCategoryDto>>>
{
    public async Task<ApiResult<IEnumerable<PostsByNonStaticPageCategoryDto>>> Handle(GetPostsByNonStaticPageCategoryQuery query, CancellationToken cancellationToken)
    {
        var result = new ApiResult<IEnumerable<PostsByNonStaticPageCategoryDto>>();
        const string methodName = nameof(GetPostsByNonStaticPageCategoryQuery);
        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving posts by non-static page categories", methodName);

            var cacheKey = CacheKeyHelper.Post.GetPostsByNonStaticPageCategoryKey();
            var cached = await cacheService.GetAsync<IEnumerable<PostsByNonStaticPageCategoryDto>>(cacheKey, cancellationToken);
            if (cached != null)
            {
                logger.Information("END {MethodName} - Successfully retrieved posts by non-static page categories from cache", methodName);
                result.Success(cached);
                return result;
            }

            var nonStaticPageCategories = await categoryGrpcClient.GetAllNonStaticPageCategories();
            
            var data = new List<PostsByNonStaticPageCategoryDto>();
            
            foreach (var category in nonStaticPageCategories)
            {
                var posts = await postRepository.GetPostsByCategoryId(category.Id, query.Count);
                var postList = posts.ToList();
                if (postList.Count != 0)
                {
                    var postSummaries = postList.Select(post => new PostDto
                    {
                        Id = post.Id,
                        Title = post.Title,
                        Slug = post.Slug,
                        ThumbnailFileId = post.ThumbnailFileId,
                        PublishedDate = post.PublishedDate,
                        ViewCount = post.ViewCount
                    }).ToList();

                    var categoryWithPosts = new PostsByNonStaticPageCategoryDto
                    {
                        Category = category,
                        Posts = postSummaries
                    };

                    data.Add(categoryWithPosts);
                }
            }

            result.Success(data);

            // Save cache (Lưu cache)
            await cacheService.SetAsync(cacheKey, data, cancellationToken: cancellationToken);

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