using MediatR;
using Microsoft.AspNetCore.Http;
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