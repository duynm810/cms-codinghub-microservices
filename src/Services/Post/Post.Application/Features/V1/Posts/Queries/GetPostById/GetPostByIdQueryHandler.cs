using AutoMapper;
using Contracts.Commons.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Domain.GrpcClients;
using Post.Domain.Repositories;
using Serilog;
using Shared.Constants;
using Shared.Dtos.Category;
using Shared.Dtos.Post.Queries;
using Shared.Helpers;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetPostById;

public class GetPostByIdQueryHandler(
    IPostRepository postRepository,
    ICategoryGrpcClient categoryGrpcClient,
    ICacheService cacheService,
    IMapper mapper,
    ILogger logger)
    : IRequestHandler<GetPostByIdQuery, ApiResult<PostDto>>
{
    public async Task<ApiResult<PostDto>> Handle(GetPostByIdQuery query, CancellationToken cancellationToken)
    {
        var result = new ApiResult<PostDto>();
        const string methodName = nameof(GetPostByIdQuery);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving post with ID: {PostId}", methodName, query.Id);

            var cacheKey = CacheKeyHelper.Post.GetPostByIdKey(query.Id);
            var cachedPost = await cacheService.GetAsync<PostDto>(cacheKey, cancellationToken);
            if (cachedPost != null)
            {
                logger.Information("END {MethodName} - Successfully retrieved post from cache with ID: {PostId}", methodName, query.Id);
                result.Success(cachedPost);
                return result;
            }

            var post = await postRepository.GetPostById(query.Id);
            if (post == null)
            {
                logger.Warning("{MethodName} - Post not found with ID: {PostId}", methodName, query.Id);
                result.Messages.Add(ErrorMessagesConsts.Post.PostNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }
            
            var data = mapper.Map<PostDto>(post);

            var category = await categoryGrpcClient.GetCategoryById(post.CategoryId);
            if (category == null)
            {
                result.Messages.Add(ErrorMessagesConsts.Category.CategoryNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            data.Category = mapper.Map<CategoryDto>(category);
            
            result.Success(data);
            await cacheService.SetAsync(cacheKey, data, cancellationToken: cancellationToken);

            logger.Information("END {MethodName} - Successfully retrieved post with ID: {PostId}", methodName, query.Id);
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