using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Application.Commons.Mappings.Interfaces;
using Post.Application.Commons.Models;
using Post.Domain.Entities;
using Post.Domain.GrpcServices;
using Post.Domain.Repositories;
using Serilog;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetPosts;

public class GetPostsQueryHandler(
    IPostRepository postRepository,
    ICategoryGrpcService categoryGrpcService,
    IMappingHelper mappingHelper,
    ILogger logger)
    : IRequestHandler<GetPostsQuery, ApiResult<IEnumerable<PostModel>>>
{
    public async Task<ApiResult<IEnumerable<PostModel>>> Handle(GetPostsQuery request,
        CancellationToken cancellationToken)
    {
        var result = new ApiResult<IEnumerable<PostModel>>();
        const string methodName = nameof(GetPostsQuery);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving all posts", methodName);

            var posts = await postRepository.GetPosts();

            var postList = posts.ToList();
            if (postList.IsNotNullOrEmpty())
            {
                var categoryIds = postList.Select(p => p.CategoryId).Distinct().ToList();
                var categories = await categoryGrpcService.GetCategoriesByIds(categoryIds);

                var data = mappingHelper.MapPostsWithCategories(postList, categories);
                result.Success(data);

                logger.Information("END {MethodName} - Successfully retrieved {PostCount} posts", methodName,
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