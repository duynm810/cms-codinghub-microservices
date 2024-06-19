using AutoMapper;
using Contracts.Commons.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Application.Commons.Mappings.Interfaces;
using Post.Domain.GrpcClients;
using Post.Domain.Repositories;
using Serilog;
using Shared.Constants;
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
    IMappingHelper mappingHelper,
    ILogger logger)
    : IRequestHandler<GetPostBySlugQuery, ApiResult<PostDetailDto>>
{
    public async Task<ApiResult<PostDetailDto>> Handle(GetPostBySlugQuery request,
        CancellationToken cancellationToken)
    {
        var result = new ApiResult<PostDetailDto>();
        const string methodName = nameof(GetPostBySlugQuery);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving post with slug: {PostSlug}", methodName, request.Slug);

            // Check existed cache (Kiểm tra cache)
            var cacheKey = CacheKeyHelper.Post.GetPostBySlugKey(request.Slug);
            var cachedPost = await cacheService.GetAsync<PostDetailDto>(cacheKey, cancellationToken);
            if (cachedPost != null)
            {
                result.Success(cachedPost);
                logger.Information("END {MethodName} - Successfully retrieved post from cache with slug: {PostSlug}",
                    methodName, request.Slug);
                return result;
            }

            var post = await postRepository.GetPostBySlug(request.Slug);
            if (post == null)
            {
                logger.Warning("{MethodName} - Post not found with slug: {PostSlug}", methodName, request.Slug);
                result.Messages.Add(ErrorMessagesConsts.Post.PostNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            // Get category and related posts at the same time (Lấy danh mục và bài viết liên quan đồng thời)
            var categoryTask = categoryGrpcClient.GetCategoryById(post.CategoryId);
            var relatedPostsTask = postRepository.GetRelatedPosts(post, request.RelatedCount);

            await Task.WhenAll(categoryTask, relatedPostsTask);

            var category = categoryTask.Result;
            if (category == null)
            {
                result.Messages.Add(ErrorMessagesConsts.Category.CategoryNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var postDto = mapper.Map<PostDto>(post);
            mapper.Map(category, postDto);
            
            var data = new PostDetailDto()
            {
                DetailPost = postDto
            };

            // Get tag information belongs to the post (Lấy thông tin các tag thuộc bài viết) 
            var tagIds = await postInTagGrpcClient.GetTagIdsByPostIdAsync(post.Id);
            var tagIdList = tagIds.ToList();
            if (tagIdList.IsNotNullOrEmpty())
            {
                var tagsInfo = await tagGrpcClient.GetTagsByIds(tagIdList);
                if (tagsInfo != null)
                {
                    var tagList = tagsInfo.ToList();
                    if (tagList.IsNotNullOrEmpty())
                    {
                        data.DetailPost.Tags = tagList.ToList();
                    }
                }
            }

            // Get user information belongs to the post (Lấy thông tin tác giả bài viết)
            var authorUserInfo = await identityGrpcClient.GetUserInfo(post.AuthorUserId);
            if (authorUserInfo != null)
            {
                data.DetailPost.User = authorUserInfo;
            }

            var relatedPosts = relatedPostsTask.Result.ToList();
            if (relatedPosts.IsNotNullOrEmpty())
            {
                var categoryIds = relatedPosts.Select(p => p.CategoryId).Distinct().ToList();
                var categories = await categoryGrpcClient.GetCategoriesByIds(categoryIds);
                var categoryDictionary = categories.ToDictionary(c => c.Id, c => c);

                var relatedPostDtos = mapper.Map<List<PostDto>>(relatedPosts);

                foreach (var relatedPost in relatedPostDtos)
                {
                    if (categoryDictionary.TryGetValue(relatedPost.Category.Id, out var relatedCategory))
                    {
                        relatedPost.Category = relatedCategory;
                    }
                }

                data.RelatedPosts = relatedPostDtos;
            }

            result.Success(data);

            // Save cache (Lưu cache)
            await cacheService.SetAsync(cacheKey, data, cancellationToken: cancellationToken);

            logger.Information("END {MethodName} - Successfully retrieved post with slug: {PostSlug}", methodName,
                request.Slug);
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