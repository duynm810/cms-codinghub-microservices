using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Application.Commons.Mappings.Interfaces;
using Post.Application.Commons.Models;
using Post.Domain.GrpcServices;
using Post.Domain.Repositories;
using Serilog;
using Shared.Responses;
using Shared.Settings;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetFeaturedPosts;

public class GetFeaturedPostsQueryHandler(
    IPostRepository postRepository,
    ICategoryGrpcService categoryGrpcService,
    PostDisplaySettings postDisplaySettings,
    IMappingHelper mappingHelper,
    ILogger logger) : IRequestHandler<GetFeaturedPostsQuery, ApiResult<IEnumerable<PostModel>>>
{
    public async Task<ApiResult<IEnumerable<PostModel>>> Handle(GetFeaturedPostsQuery request,
        CancellationToken cancellationToken)
    {
        var result = new ApiResult<IEnumerable<PostModel>>();
        const string methodName = nameof(GetFeaturedPostsQuery);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving featured posts", methodName);

            var posts = await postRepository.GetFeaturedPosts(postDisplaySettings.FeaturedPostsCount);

            var postList = posts.ToList();
            
            if (postList.IsNotNullOrEmpty())
            {
                var categoryIds = postList.Select(p => p.CategoryId).Distinct().ToList();
                var categories = await categoryGrpcService.GetCategoriesByIds(categoryIds);

                var data = mappingHelper.MapPostsWithCategories(postList, categories);
                result.Success(data);

                logger.Information("END {MethodName} - Successfully retrieved {PostCount} featured posts", methodName,
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