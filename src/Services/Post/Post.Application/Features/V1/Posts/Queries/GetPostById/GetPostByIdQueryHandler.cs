using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Application.Commons.Models;
using Post.Domain.Interfaces;
using Serilog;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetPostById;

public class GetPostByIdQueryHandler(IPostRepository postRepository, ICategoryGrpcService categoryGrpcService, IMapper mapper, ILogger logger)
    : IRequestHandler<GetPostByIdQuery, ApiResult<PostDto>>
{
    public async Task<ApiResult<PostDto>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<PostDto>();

        try
        {
            var post = await postRepository.GetPostById(request.Id);
            if (post == null)
            {
                result.Messages.Add("Post not found");
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }
            
            var data = mapper.Map<PostDto>(post);
            
            var category = await categoryGrpcService.GetCategoryById(post.CategoryId);
            if (category != null)
            {
                data.CategoryName = category.Name;
            }
            
            result.Success(data);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(GetPostByIdQuery), e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }
}