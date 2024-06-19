using Contracts.Commons.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Domain.GrpcClients;
using Post.Domain.Repositories;
using Post.Domain.Services;
using Serilog;
using Shared.Dtos.Post.Queries;
using Shared.Helpers;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsByAuthorPaging;

public class GetPostsByAuthorPagingQueryHandler(
    IPostRepository postRepository,
    IIdentityGrpcClient identityGrpcClient,
    ICacheService cacheService,
    IPostService postService,
    ILogger logger) : IRequestHandler<GetPostsByAuthorPagingQuery, ApiResult<PostsByAuthorDto>>
{
    public async Task<ApiResult<PostsByAuthorDto>> Handle(GetPostsByAuthorPagingQuery request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<PostsByAuthorDto>();
        const string methodName = nameof(GetPostsByAuthorPagingQuery);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving posts for author {AuthorId} on page {PageNumber} with page size {PageSize}", methodName, request.AuthorId, request.PageNumber, request.PageSize);
            
            var cacheKey = CacheKeyHelper.Post.GetPostsByAuthorPagingKey(request.AuthorId.ToString(), request.PageNumber, request.PageSize);
            var cachedPosts = await cacheService.GetAsync<PostsByAuthorDto>(cacheKey, cancellationToken);
            if (cachedPosts != null)
            {
                logger.Information("END {MethodName} - Successfully retrieved posts from cache for author {AuthorId} on page {PageNumber} with page size {PageSize}", methodName, request.AuthorId, request.PageNumber, request.PageSize);
                result.Success(cachedPosts);
                return result;
            }
            
            // Get user belongs to the post (Lấy tác giả bài viết)
            var authorUserInfo = await identityGrpcClient.GetUserInfo(request.AuthorId);
            
            var data = new PostsByAuthorDto { User = authorUserInfo };
            
            var posts = await postRepository.GetPostsByAuthorPaging(request.AuthorId, request.PageNumber, request.PageSize);
            if (posts.Items != null && posts.Items.IsNotNullOrEmpty())
            {
                data.Posts = await postService.EnrichPagedPostsWithCategories(posts, cancellationToken);
                
                result.Success(data);

                // Save cache (Lưu cache)
                await cacheService.SetAsync(cacheKey, data.Posts.Items, cancellationToken: cancellationToken);

                logger.Information("END {MethodName} - Successfully retrieved {PostCount} posts for author {AuthorId} for page {PageNumber} with page size {PageSize}", methodName, data.Posts.MetaData.TotalItems, request.AuthorId, request.PageNumber, request.PageSize);
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