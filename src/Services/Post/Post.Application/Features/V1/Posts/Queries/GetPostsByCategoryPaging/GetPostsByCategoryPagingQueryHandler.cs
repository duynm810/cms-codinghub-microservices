using AutoMapper;
using Contracts.Commons.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Domain.GrpcClients;
using Post.Domain.Repositories;
using Serilog;
using Shared.Constants;
using Shared.Dtos.Category;
using Shared.Dtos.Post;
using Shared.Helpers;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsByCategoryPaging;

public class GetPostsByCategoryPagingQueryHandler(
    IPostRepository postRepository,
    ICategoryGrpcClient categoryGrpcClient,
    ICacheService cacheService,
    IMapper mapper,
    ILogger logger) : IRequestHandler<GetPostsByCategoryPagingQuery, ApiResult<PostsByCategoryDto>>
{
    public async Task<ApiResult<PostsByCategoryDto>> Handle(GetPostsByCategoryPagingQuery query,
        CancellationToken cancellationToken)
    {
        var result = new ApiResult<PostsByCategoryDto>();
        const string methodName = nameof(GetPostsByCategoryPagingQuery);

        try
        {
            logger.Information(
                "BEGIN {MethodName} - Retrieving posts for category slug {CategorySlug} on page {PageNumber} with page size {PageSize}",
                methodName, query.CategorySlug, query.Request.PageNumber, query.Request.PageSize);

            var cacheKey = CacheKeyHelper.Post.GetPostsByCategoryPagingKey(query.CategorySlug, query.Request.PageNumber, query.Request.PageSize);
            var cached = await cacheService.GetAsync<PostsByCategoryDto>(cacheKey, cancellationToken);
            if (cached != null)
            {
                logger.Information(
                    "END {MethodName} - Successfully retrieved posts from cache for category slug {CategorySlug} on page {PageNumber} with page size {PageSize}",
                    methodName, query.CategorySlug, query.Request.PageNumber, query.Request.PageSize);
                result.Success(cached);
                return result;
            }

            var category = await categoryGrpcClient.GetCategoryBySlug(query.CategorySlug);
            if (category == null)
            {
                logger.Warning("{MethodName} - Category not found with slug: {CategorySlug}", methodName,
                    query.CategorySlug);
                result.Messages.Add(ErrorMessagesConsts.Category.CategoryNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var pagedPosts =
                await postRepository.GetPostsByCategoryPaging(category.Id, query.Request);

            var posts = mapper.Map<List<PostDto>>(pagedPosts.Items);

            foreach (var post in posts)
            {
                post.Category = mapper.Map<CategoryDto>(category);
            }

            var data = new PostsByCategoryDto
            {
                Category = category,
                Posts = new PagedResponse<PostDto>()
                {
                    Items = posts,
                    MetaData = pagedPosts.MetaData
                }
            };

            result.Success(data);

            await cacheService.SetAsync(cacheKey, data, cancellationToken: cancellationToken);

            logger.Information(
                "END {MethodName} - Successfully retrieved {PostCount} posts for category slug {CategorySlug} on page {PageNumber} with page size {PageSize}",
                methodName, data.Posts.MetaData.TotalItems, query.CategorySlug, query.Request.PageNumber,
                query.Request.PageSize);
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