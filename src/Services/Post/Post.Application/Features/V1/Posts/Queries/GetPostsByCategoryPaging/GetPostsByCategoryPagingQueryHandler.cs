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

namespace Post.Application.Features.V1.Posts.Queries.GetPostsByCategoryPaging;

public class GetPostsByCategoryPagingQueryHandler(
    IPostRepository postRepository,
    ICategoryGrpcClient categoryGrpcClient,
    ICacheService cacheService,
    IMapper mapper,
    ILogger logger) : IRequestHandler<GetPostsByCategoryPagingQuery, ApiResult<PagedResponse<PostDto>>>
{
    public async Task<ApiResult<PagedResponse<PostDto>>> Handle(GetPostsByCategoryPagingQuery request,
        CancellationToken cancellationToken)
    {
        var result = new ApiResult<PagedResponse<PostDto>>();
        const string methodName = nameof(GetPostsByCategoryPagingQuery);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving posts for category slug {CategorySlug} on page {PageNumber} with page size {PageSize}", methodName, request.CategorySlug, request.PageNumber, request.PageSize);
            
            var cacheKey = CacheKeyHelper.Post.GetPostsByCategoryPagingKey(request.CategorySlug, request.PageNumber, request.PageSize);
            var cachedPosts = await cacheService.GetAsync<PagedResponse<PostDto>>(cacheKey, cancellationToken);
            if (cachedPosts != null)
            {
                logger.Information("END {MethodName} - Successfully retrieved posts from cache for category slug {CategorySlug} on page {PageNumber} with page size {PageSize}", methodName, request.CategorySlug, request.PageNumber, request.PageSize);
                result.Success(cachedPosts);
                return result;
            }

            var category = await categoryGrpcClient.GetCategoryBySlug(request.CategorySlug);
            if (category == null)
            {
                logger.Warning("{MethodName} - Category not found with slug: {CategorySlug}", methodName, request.CategorySlug);
                result.Messages.Add(ErrorMessagesConsts.Category.CategoryNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var pagedPosts = await postRepository.GetPostsByCategoryPaging(category.Id, request.PageNumber, request.PageSize);
            
            var posts = mapper.Map<List<PostDto>>(pagedPosts.Items);
            
            foreach (var post in posts)
            {
                post.Category = mapper.Map<CategoryDto>(category);
            }

            var data = new PagedResponse<PostDto>
            {
                Items = posts,
                MetaData = pagedPosts.MetaData
            };
            
            result.Success(data);

            // Save cache (Lưu cache)
            await cacheService.SetAsync(cacheKey, data, cancellationToken: cancellationToken);

            logger.Information(
                "END {MethodName} - Successfully retrieved {PostCount} posts for category slug {CategorySlug} on page {PageNumber} with page size {PageSize}",
                methodName, data.MetaData.TotalItems, request.CategorySlug, request.PageNumber, request.PageSize);
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