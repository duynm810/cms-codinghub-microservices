using AutoMapper;
using Contracts.Commons.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Domain.GrpcClients;
using Post.Domain.Repositories;
using Post.Domain.Services;
using Serilog;
using Shared.Constants;
using Shared.Dtos.Category;
using Shared.Dtos.Post;
using Shared.Helpers;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetDetailBySlug;

public class GetDetailBySlugQueryHandler(
    IPostRepository postRepository,
    ICategoryGrpcClient categoryGrpcClient,
    ITagGrpcClient tagGrpcClient,
    IPostInTagGrpcClient postInTagGrpcClient,
    IIdentityGrpcClient identityGrpcClient,
    ICacheService cacheService,
    IPostService postService,
    IMapper mapper,
    ILogger logger)
    : IRequestHandler<GetDetailBySlugQuery, ApiResult<PostsBySlugDto>>
{
    public async Task<ApiResult<PostsBySlugDto>> Handle(GetDetailBySlugQuery query,
        CancellationToken cancellationToken)
    {
        var result = new ApiResult<PostsBySlugDto>();
        const string methodName = nameof(GetDetailBySlugQuery);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving post with slug: {PostSlug}", methodName, query.Slug);

            var cacheKey = CacheKeyHelper.Post.GetDetailBySlugKey(query.Slug);
            var cachedPost = await cacheService.GetAsync<PostsBySlugDto>(cacheKey, cancellationToken);
            if (cachedPost != null)
            {
                logger.Information("Retrieved post from cache with slug: {PostSlug}", query.Slug);
                result.Success(cachedPost);
                return result;
            }

            var post = await postRepository.GetPostBySlug(query.Slug);
            if (post == null)
            {
                logger.Warning("{MethodName} - Post not found with slug: {PostSlug}", methodName, query.Slug);
                result.Messages.Add(ErrorMessagesConsts.Post.PostNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var postDetail = mapper.Map<PostDto>(post);

            // Fetch category, related posts, tag ids, and author user info in parallel
            var categoryTask = categoryGrpcClient.GetCategoryById(post.CategoryId);
            var relatedPostsTask = postRepository.GetRelatedPosts(post, query.RelatedCount);
            var tagIdsTask = postInTagGrpcClient.GetTagIdsByPostIdAsync(post.Id);
            var authorUserInfoTask = identityGrpcClient.GetUserInfo(post.AuthorUserId);

            await Task.WhenAll(categoryTask, relatedPostsTask, tagIdsTask, authorUserInfoTask);

            var category = await categoryTask;
            if (category == null)
            {
                result.Messages.Add(ErrorMessagesConsts.Category.CategoryNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            postDetail.Category = mapper.Map<CategoryDto>(category);

            var data = new PostsBySlugDto { Detail = postDetail };

            var tagIds = await tagIdsTask;
            var tagIdList = tagIds.ToList();
            if (tagIdList.IsNotNullOrEmpty())
            {
                var tagsInfo = await tagGrpcClient.GetTagsByIds(tagIdList);
                if (tagsInfo != null)
                {
                    data.Detail.Tags = tagsInfo.ToList();
                }
            }

            var authorUserInfo = await authorUserInfoTask;
            if (authorUserInfo != null)
            {
                data.Detail.User = authorUserInfo;
            }

            var relatedPosts = await relatedPostsTask;
            var postBases = relatedPosts.ToList();
            if (postBases.IsNotNullOrEmpty())
            {
                var relatedPostList = await postService.EnrichPostsWithCategories(postBases, cancellationToken);
                data.RelatedPosts = relatedPostList;
            }

            result.Success(data);
            await cacheService.SetAsync(cacheKey, data, cancellationToken: cancellationToken);

            logger.Information("END {MethodName} - Successfully retrieved post with slug: {PostSlug}", methodName, query.Slug);
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