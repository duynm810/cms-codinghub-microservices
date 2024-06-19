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
    ITagGrpcClient tagGrpcClient,
    IPostInTagGrpcClient postInTagGrpcClient,
    IIdentityGrpcClient identityGrpcClient,
    ICacheService cacheService,
    IMapper mapper,
    ILogger logger)
    : IRequestHandler<GetPostBySlugQuery, ApiResult<PostBySlugDto>>
{
    public async Task<ApiResult<PostBySlugDto>> Handle(GetPostBySlugQuery request,
        CancellationToken cancellationToken)
    {
        var result = new ApiResult<PostBySlugDto>();
        const string methodName = nameof(GetPostBySlugQuery);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving post with slug: {PostSlug}", methodName, request.Slug);

            var cacheKey = CacheKeyHelper.Post.GetPostBySlugKey(request.Slug);
            var cachedPost = await cacheService.GetAsync<PostBySlugDto>(cacheKey, cancellationToken).ConfigureAwait(false);
            if (cachedPost != null)
            {
                logger.Information("Retrieved post from cache with slug: {PostSlug}", request.Slug);
                result.Success(cachedPost);
                return result;
            }

            var post = await postRepository.GetPostBySlug(request.Slug).ConfigureAwait(false);
            if (post == null)
            {
                logger.Warning("{MethodName} - Post not found with slug: {PostSlug}", methodName, request.Slug);
                result.Messages.Add(ErrorMessagesConsts.Post.PostNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var postDetail = mapper.Map<PostDto>(post);

            var categoryTask = categoryGrpcClient.GetCategoryById(post.CategoryId);
            var relatedPostsTask = postRepository.GetRelatedPosts(post, request.RelatedCount);

            await Task.WhenAll(categoryTask, relatedPostsTask).ConfigureAwait(false);

            var category = await categoryTask.ConfigureAwait(false);
            if (category == null)
            {
                result.Messages.Add(ErrorMessagesConsts.Category.CategoryNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            postDetail.Category = mapper.Map<CategoryDto>(category);
            var data = new PostBySlugDto { Detail = postDetail };

            var tagIds = await postInTagGrpcClient.GetTagIdsByPostIdAsync(post.Id).ConfigureAwait(false);
            var tagIdList = tagIds.ToList();
            if (tagIdList.IsNotNullOrEmpty())
            {
                var tagsInfo = await tagGrpcClient.GetTagsByIds(tagIdList).ConfigureAwait(false);
                if (tagsInfo != null)
                {
                    data.Detail.Tags = tagsInfo.ToList();
                }
            }

            var authorUserInfo = await identityGrpcClient.GetUserInfo(post.AuthorUserId).ConfigureAwait(false);
            if (authorUserInfo != null)
            {
                data.Detail.User = authorUserInfo;
            }

            var relatedPosts = relatedPostsTask.Result.ToList();
            if (relatedPosts.IsNotNullOrEmpty())
            {
                var categoryIds = relatedPosts.Select(p => p.CategoryId).Distinct().ToList();
                var categories = await categoryGrpcClient.GetCategoriesByIds(categoryIds).ConfigureAwait(false);
                var categoryDictionary = categories.ToDictionary(c => c.Id, c => c);

                var relatedPostDtos = mapper.Map<List<PostDto>>(relatedPosts);
                foreach (var relatedPost in relatedPostDtos)
                {
                    if (categoryDictionary.TryGetValue(relatedPost.CategoryId, out var relatedCategory))
                    {
                        relatedPost.Category = relatedCategory;
                    }
                }

                data.RelatedPosts = relatedPostDtos;
            }

            result.Success(data);
            await cacheService.SetAsync(cacheKey, data, cancellationToken: cancellationToken).ConfigureAwait(false);

            logger.Information("END {MethodName} - Successfully retrieved post with slug: {PostSlug}", methodName, request.Slug);
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