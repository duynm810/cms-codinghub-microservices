using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Application.Commons.Models;
using Post.Domain.GrpcServices;
using Post.Domain.Repositories;
using Serilog;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsPaging;

public class GetPostsPagingQueryHandler(IPostRepository postRepository, ICategoryGrpcService categoryGrpcService, IMapper mapper, ILogger logger) : IRequestHandler<GetPostsPagingQuery, ApiResult<PagedResponse<PostModel>>>
{
    public async Task<ApiResult<PagedResponse<PostModel>>> Handle(GetPostsPagingQuery request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<PagedResponse<PostModel>>();
        const string methodName = nameof(GetPostsPagingQuery);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving posts for page {PageNumber} with page size {PageSize}", methodName, request.PageNumber, request.PageSize);

            var posts = await postRepository.GetPostsPaging(request.Filter, request.PageNumber, request.PageSize);

            if (posts.Items != null && posts.Items.Count != 0)
            {
                var categoryIds = posts.Items.Select(p => p.CategoryId).Distinct().ToList();
                var categories = await categoryGrpcService.GetCategoriesByIds(categoryIds);
                var categoryDictionary = categories.ToDictionary(c => c.Id, c => c);
                
                var postList = mapper.Map<List<PostModel>>(posts.Items);
                foreach (var post in postList)
                {
                    if (!categoryDictionary.TryGetValue(post.CategoryId, out var category))
                    {
                        continue;
                    }
                    
                    post.CategoryName = category.Name;
                    post.CategorySlug = category.Slug;
                    post.CategoryIcon = category.Icon;
                    post.CategoryColor = category.Color;
                }
                
                var data = new PagedResponse<PostModel>()
                {
                    Items = postList,
                    MetaData = posts.MetaData
                };
                
                result.Success(data);
                
                logger.Information("END {MethodName} - Successfully retrieved {PostCount} posts for page {PageNumber} with page size {PageSize}", methodName, postList.Count, request.PageNumber, request.PageSize);
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