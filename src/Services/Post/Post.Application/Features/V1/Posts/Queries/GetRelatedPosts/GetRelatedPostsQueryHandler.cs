using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Application.Commons.Models;
using Post.Domain.GrpcServices;
using Post.Domain.Repositories;
using Serilog;
using Shared.Constants;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetRelatedPosts;

public class GetRelatedPostsQueryHandler(
    IPostRepository postRepository,
    ICategoryGrpcService categoryGrpcService,
    IMapper mapper,
    ILogger logger) : IRequestHandler<GetRelatedPostsQuery, ApiResult<IEnumerable<PostDto>>>
{
    public async Task<ApiResult<IEnumerable<PostDto>>> Handle(GetRelatedPostsQuery request,
        CancellationToken cancellationToken)
    {
        var result = new ApiResult<IEnumerable<PostDto>>();
        const string methodName = nameof(GetRelatedPostsQuery);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving related posts", methodName);

            var post = await postRepository.GetPostById(request.Id);
            if (post == null)
            {
                logger.Warning("{MethodName} - Post not found with ID: {PostId}", methodName, request.Id);
                result.Messages.Add(ErrorMessagesConsts.Post.PostNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }
            
            var relatedPost = await postRepository.GetRelatedPosts(post, request.Count);

            var postBases = relatedPost.ToList();
            if (postBases.IsNotNullOrEmpty())
            {
                var categoryIds = postBases.Select(p => p.CategoryId).Distinct().ToList();
                var categories = await categoryGrpcService.GetCategoriesByIds(categoryIds);
                var categoryDictionary = categories.ToDictionary(c => c.Id, c => c);

                var data = mapper.Map<List<PostDto>>(relatedPost);
                foreach (var item in data)
                {
                    if (!categoryDictionary.TryGetValue(item.CategoryId, out var category))
                    {
                        continue;
                    }

                    item.CategoryName = category.Name;
                    item.CategorySlug = category.Slug;
                    item.CategoryIcon = category.Icon;
                    item.CategoryColor = category.Color;
                }

                result.Success(data);

                logger.Information("END {MethodName} - Successfully retrieved {PostCount} related posts", methodName,
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