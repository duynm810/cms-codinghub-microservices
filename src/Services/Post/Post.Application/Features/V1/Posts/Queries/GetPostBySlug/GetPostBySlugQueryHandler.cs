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

namespace Post.Application.Features.V1.Posts.Queries.GetPostBySlug;

public class GetPostBySlugQueryHandler(
    IPostRepository postRepository,
    ICategoryGrpcService categoryGrpcService,
    IMapper mapper,
    ILogger logger)
    : IRequestHandler<GetPostBySlugQuery, ApiResult<PostDto>>
{
    public async Task<ApiResult<PostDto>> Handle(GetPostBySlugQuery request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<PostDto>();
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

            var data = mapper.Map<PostDto>(post);

            var category = await categoryGrpcService.GetCategoryById(post.CategoryId);
            if (category != null)
            {
                data.CategoryName = category.Name;
                data.CategorySlug = category.Slug;
                data.CategoryIcon = category.Icon;
                data.CategoryColor = category.Color;
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