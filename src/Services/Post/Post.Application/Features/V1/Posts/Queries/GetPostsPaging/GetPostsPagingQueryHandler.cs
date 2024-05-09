using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Application.Commons.Models;
using Post.Domain.Interfaces;
using Serilog;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsPaging;

public class GetPostsPagingQueryHandler(IPostRepository postRepository, ICategoryGrpcService categoryGrpcService, IMapper mapper, ILogger logger) : IRequestHandler<GetPostsPagingQuery, ApiResult<PagedResponse<PostDto>>>
{
    public async Task<ApiResult<PagedResponse<PostDto>>> Handle(GetPostsPagingQuery request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<PagedResponse<PostDto>>();

        try
        {
            var posts = await postRepository.GetPostsPaging(request.PageNumber, request.PageSize);

            if (posts.Items != null && posts.Items.Count != 0)
            {
                var categoryIds = posts.Items.Select(p => p.CategoryId).Distinct().ToList();
                var categories = await categoryGrpcService.GetCategoriesByIds(categoryIds);
                var categoryDictionary = categories.ToDictionary(c => c.Id, c => c.Name);
                
                var postDtos = mapper.Map<List<PostDto>>(posts.Items);
                foreach (var post in postDtos)
                {
                    if (categoryDictionary.TryGetValue(post.CategoryId, out var value))
                    {
                        post.CategoryName = value;
                    }
                }
                
                var data = new PagedResponse<PostDto>()
                {
                    Items = postDtos,
                    MetaData = posts.MetaData
                };
                
                result.Success(data);
            }
        }
        catch (Exception e)
        {
            logger.Error("Method: {MethodName}. Message: {ErrorMessage}", nameof(GetPostsPagingQuery), e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }
}