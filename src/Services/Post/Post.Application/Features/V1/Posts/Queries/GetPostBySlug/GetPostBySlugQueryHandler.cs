using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Application.Commons.Models;
using Post.Domain.GrpcServices;
using Post.Domain.Repositories;
using Serilog;
using Shared.Constants;
using Shared.Responses;
using Shared.Settings;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetPostBySlug;

public class GetPostBySlugQueryHandler(
    IPostRepository postRepository,
    ICategoryGrpcService categoryGrpcService,
    PostDisplaySettings postDisplaySettings,
    IMapper mapper,
    ILogger logger)
    : IRequestHandler<GetPostBySlugQuery, ApiResult<PostDetailModel>>
{
    public async Task<ApiResult<PostDetailModel>> Handle(GetPostBySlugQuery request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<PostDetailModel>();
        const string methodName = nameof(GetPostBySlugQuery);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving post with slug: {PostSlug}", methodName, request.Slug);

            var post = await postRepository.GetPostBySlug(request.Slug);
            if (post == null)
            {
                logger.Warning("{MethodName} - Post not found with slug: {PostSlug}", methodName, request.Slug);
                result.Messages.Add(ErrorMessagesConsts.Post.PostNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var data = new PostDetailModel
            {
                DetailPost = mapper.Map<PostModel>(post)
            };
            
            // Lấy danh mục và bài viết liên quan đồng thời
            var categoryTask = categoryGrpcService.GetCategoryById(post.CategoryId);
            var relatedPostsTask =  postRepository.GetRelatedPosts(post, postDisplaySettings.RelatedPostsCount);

            await Task.WhenAll(categoryTask, relatedPostsTask);

            var category = categoryTask.Result;
            if (category != null)
            {
                data.DetailPost.CategoryName = category.Name;
                data.DetailPost.CategorySlug = category.Slug;
                data.DetailPost.CategoryIcon = category.Icon;
                data.DetailPost.CategoryColor = category.Color;
            }
            
            var relatedPosts = relatedPostsTask.Result.ToList();
            if (relatedPosts.IsNotNullOrEmpty())
            {
                var categoryIds = relatedPosts.Select(p => p.CategoryId).Distinct().ToList();
                var categories = await categoryGrpcService.GetCategoriesByIds(categoryIds);
                var categoryDictionary = categories.ToDictionary(c => c.Id, c => c);

                var relatedPostDtos = mapper.Map<List<PostModel>>(relatedPosts);
                foreach (var item in relatedPostDtos)
                {
                    if (!categoryDictionary.TryGetValue(item.CategoryId, out var relatedCategory))
                    {
                        continue;
                    }

                    item.CategoryName = relatedCategory.Name;
                    item.CategorySlug = relatedCategory.Slug;
                    item.CategoryIcon = relatedCategory.Icon;
                    item.CategoryColor = relatedCategory.Color;
                }

                data.RelatedPosts = relatedPostDtos;
            }

            result.Success(data);

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