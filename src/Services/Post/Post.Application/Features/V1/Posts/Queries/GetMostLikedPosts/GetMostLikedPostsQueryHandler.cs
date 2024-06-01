using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Application.Commons.Mappings.Interfaces;
using Post.Application.Commons.Models;
using Post.Domain.GrpcServices;
using Post.Domain.Repositories;
using Serilog;
using Shared.Constants;
using Shared.Responses;
using Shared.Settings;

namespace Post.Application.Features.V1.Posts.Queries.GetMostLikedPosts;

public class GetMostLikedPostsQueryHandler(
    IPostRepository postRepository,
    ICategoryGrpcService categoryGrpcService,
    DisplaySettings displaySettings,
    IMappingHelper mappingHelper,
    ILogger logger) : IRequestHandler<GetMostLikedPostsQuery, ApiResult<IEnumerable<PostModel>>>
{
    public async Task<ApiResult<IEnumerable<PostModel>>> Handle(GetMostLikedPostsQuery request,
        CancellationToken cancellationToken)
    {
        var result = new ApiResult<IEnumerable<PostModel>>();
        const string methodName = nameof(GetMostLikedPostsQuery);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving most liked posts", methodName);

            var posts = await postRepository.GetMostLikedPosts(
                displaySettings.Config.GetValueOrDefault(DisplaySettingsConsts.Post.MostLikedPosts, 0));

            var postList = posts.ToList();
            
            if (postList.Any())
            {
                var categoryIds = postList.Select(p => p.CategoryId).Distinct().ToList();
                var categories = await categoryGrpcService.GetCategoriesByIds(categoryIds);

                var data = mappingHelper.MapPostsWithCategories(postList, categories);
                result.Success(data);

                logger.Information("END {MethodName} - Successfully retrieved {PostCount} most liked posts", methodName, data.Count);
            }
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
