using AutoMapper;
using Contracts.Commons.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Post.Application.Commons.Mappings.Interfaces;
using Post.Application.Commons.Models;
using Post.Domain.GrpcClients;
using Post.Domain.Repositories;
using Serilog;
using Shared.Constants;
using Shared.Dtos.Post;
using Shared.Helpers;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetPostById;

public class GetPostByIdQueryHandler(
    IPostRepository postRepository,
    ICategoryGrpcClient categoryGrpcClient,
    ICacheService cacheService,
    IMappingHelper mappingHelper,
    ILogger logger)
    : IRequestHandler<GetPostByIdQuery, ApiResult<PostModel>>
{
    public async Task<ApiResult<PostModel>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<PostModel>();
        const string methodName = nameof(GetPostByIdQuery);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving post with ID: {PostId}", methodName, request.Id);

            // Check existed cache (Kiểm tra cache)
            var cacheKey = CacheKeyHelper.Post.GetPostByIdKey(request.Id);
            var cachedPost = await cacheService.GetAsync<PostModel>(cacheKey, cancellationToken);
            if (cachedPost != null)
            {
                result.Success(cachedPost);
                logger.Information("END {MethodName} - Successfully retrieved post from cache with ID: {PostId}",
                    methodName, request.Id);
                return result;
            }

            var post = await postRepository.GetPostById(request.Id);
            if (post == null)
            {
                logger.Warning("{MethodName} - Post not found with ID: {PostId}", methodName, request.Id);
                result.Messages.Add(ErrorMessagesConsts.Post.PostNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var category = await categoryGrpcClient.GetCategoryById(post.CategoryId);
            if (category == null)
            {
                result.Messages.Add(ErrorMessagesConsts.Category.CategoryNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var data = mappingHelper.MapPostWithCategory(post, category);
            result.Success(data);

            // Save cache (Lưu cache)
            await cacheService.SetAsync(cacheKey, data, cancellationToken: cancellationToken);

            logger.Information("END {MethodName} - Successfully retrieved post with ID: {PostId}", methodName,
                request.Id);
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