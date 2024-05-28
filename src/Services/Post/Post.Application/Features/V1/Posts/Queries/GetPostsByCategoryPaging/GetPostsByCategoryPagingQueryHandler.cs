using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Application.Commons.Models;
using Post.Domain.GrpcServices;
using Post.Domain.Repositories;
using Serilog;
using Shared.Constants;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsByCategoryPaging;

public class GetPostsByCategoryPagingQueryHandler(
    IPostRepository postRepository,
    ICategoryGrpcService categoryGrpcService,
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
            logger.Information(
                "BEGIN {MethodName} - Retrieving posts for category slug {CategorySlug} on page {PageNumber} with page size {PageSize}",
                methodName, request.CategorySlug, request.PageNumber, request.PageSize);

            var category = await categoryGrpcService.GetCategoryBySlug(request.CategorySlug);
            if (category == null)
            {
                logger.Warning("{MethodName} - Category not found with slug: {CategorySlug}", methodName,
                    request.CategorySlug);
                result.Messages.Add(ErrorMessagesConsts.Category.CategoryNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            // Lấy bài viết theo categoryId và phân trang
            var posts = await postRepository.GetPostsByCategoryPaging(category.Id, request.PageNumber,
                request.PageSize);

            var postDtos = mapper.Map<List<PostDto>>(posts.Items);
            foreach (var post in postDtos)
            {
                post.CategoryName = category.Name;
                post.CategorySlug = category.Slug;
                post.CategoryIcon = category.Icon;
                post.CategoryColor = category.Color;
            }

            var pagedResponse = new PagedResponse<PostDto>()
            {
                Items = postDtos,
                MetaData = posts.MetaData
            };

            result.Success(pagedResponse);

            logger.Information(
                "END {MethodName} - Successfully retrieved {PostCount} posts for category slug {CategorySlug} on page {PageNumber} with page size {PageSize}",
                methodName, postDtos.Count, request.CategorySlug, request.PageNumber, request.PageSize);
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