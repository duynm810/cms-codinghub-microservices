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

namespace Post.Application.Features.V1.Posts.Queries.GetPostBySlug;

public class GetPostBySlugQueryHandler(
    IPostRepository postRepository,
    ICategoryGrpcClient categoryGrpcClient,
    ICacheService cacheService,
    IMapper mapper,
    ILogger logger)
    : IRequestHandler<GetPostBySlugQuery, ApiResult<PostDto>>
{
    public async Task<ApiResult<PostDto>> Handle(GetPostBySlugQuery request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<PostDto>();
        const string methodName = nameof(GetPostBySlugQuery);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving post with Slug: {PostSlug}", methodName, request.Slug);

            var cacheKey = CacheKeyHelper.Post.GetPostBySlugKey(request.Slug);
            var cachedPost = await cacheService.GetAsync<PostDto>(cacheKey, cancellationToken);
            if (cachedPost != null)
            {
                logger.Information("END {MethodName} - Successfully retrieved post from cache with Slug: {PostSlug}", methodName, request.Slug);
                result.Success(cachedPost);
                return result;
            }

            var post = await postRepository.GetPostBySlug(request.Slug);
            if (post == null)
            {
                logger.Warning("{MethodName} - Post not found with Slug: {PostSlug}", methodName, request.Slug);
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

            logger.Information("END {MethodName} - Successfully retrieved post with Slug: {PostSlug}", methodName, request.Slug);
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