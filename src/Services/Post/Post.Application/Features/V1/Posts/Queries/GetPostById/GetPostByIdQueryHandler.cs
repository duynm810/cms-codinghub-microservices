using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Application.Commons.Mappings.Interfaces;
using Post.Application.Commons.Models;
using Post.Domain.GrpcServices;
using Post.Domain.Repositories;
using Serilog;
using Shared.Constants;
using Shared.Dtos.Post;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetPostById;

public class GetPostByIdQueryHandler(
    IPostRepository postRepository,
    ICategoryGrpcService categoryGrpcService,
    IMappingHelper mappingHelper,
    ILogger logger)
    : IRequestHandler<GetPostByIdQuery, ApiResult<PostModel>>
{
    public async Task<ApiResult<PostModel>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<PostModel>();
        const string methodName = nameof(GetPostByIdQuery);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving post with ID: {PostId}", methodName, request.Id);

            var post = await postRepository.GetPostById(request.Id);
            if (post == null)
            {
                logger.Warning("{MethodName} - Post not found with ID: {PostId}", methodName, request.Id);
                result.Messages.Add(ErrorMessagesConsts.Post.PostNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var category = await categoryGrpcService.GetCategoryById(post.CategoryId);
            if (category == null)
            {
                result.Messages.Add(ErrorMessagesConsts.Category.CategoryNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }
            
            var data = mappingHelper.MapPostWithCategory(post, category);
            result.Success(data);

            logger.Information("END {MethodName} - Successfully retrieved post with ID: {PostId}", methodName,
                request.Id);
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