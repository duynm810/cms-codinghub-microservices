using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Application.Commons.Models;
using Post.Domain.GrpcServices;
using Post.Domain.Repositories;
using Serilog;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetFeaturedPosts;

public class GetFeaturedPostsQueryHandler(
    IPostRepository postRepository,
    ICategoryGrpcService categoryGrpcService,
    IMapper mapper,
    ILogger logger) : IRequestHandler<GetFeaturedPostsQuery, ApiResult<IEnumerable<PostDto>>>
{
    public async Task<ApiResult<IEnumerable<PostDto>>> Handle(GetFeaturedPostsQuery request,
        CancellationToken cancellationToken)
    {
        var result = new ApiResult<IEnumerable<PostDto>>();
        const string methodName = nameof(GetFeaturedPostsQuery);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving featured posts", methodName);

            var posts = await postRepository.GetFeaturedPosts(request.Count);

            var postBases = posts.ToList();
            if (postBases.IsNotNullOrEmpty())
            {
                var categoryIds = postBases.Select(p => p.CategoryId).Distinct().ToList();
                var categories = await categoryGrpcService.GetCategoriesByIds(categoryIds);
                var categoryDictionary = categories.ToDictionary(c => c.Id, c => c);

                var data = mapper.Map<List<PostDto>>(posts);
                foreach (var post in data)
                {
                    if (!categoryDictionary.TryGetValue(post.CategoryId, out var category))
                    {
                        continue;
                    }
                    
                    post.CategoryName = category.Name;
                    post.CategoryIcon = category.Icon;
                    post.CategoryColor = category.Color;
                }

                result.Success(data);

                logger.Information("END {MethodName} - Successfully retrieved {PostCount} featured posts", methodName,
                    data.Count());
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