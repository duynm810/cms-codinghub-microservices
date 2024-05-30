using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Application.Commons.Mappings.Interfaces;
using Post.Application.Commons.Models;
using Post.Domain.GrpcServices;
using Post.Domain.Repositories;
using Serilog;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsPaging;

public class GetPostsPagingQueryHandler(
    IPostRepository postRepository,
    ICategoryGrpcService categoryGrpcService,
    IMappingHelper mappingHelper,
    ILogger logger) : IRequestHandler<GetPostsPagingQuery, ApiResult<PagedResponse<PostModel>>>
{
    public async Task<ApiResult<PagedResponse<PostModel>>> Handle(GetPostsPagingQuery request,
        CancellationToken cancellationToken)
    {
        var result = new ApiResult<PagedResponse<PostModel>>();
        const string methodName = nameof(GetPostsPagingQuery);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving posts for page {PageNumber} with page size {PageSize}",
                methodName, request.PageNumber, request.PageSize);

            var posts = await postRepository.GetPostsPaging(request.Filter, request.PageNumber, request.PageSize);

            if (posts.Items != null && posts.Items.IsNotNullOrEmpty())
            {
                var categoryIds = posts.Items.Select(p => p.CategoryId).Distinct().ToList();
                var categories = await categoryGrpcService.GetCategoriesByIds(categoryIds);

                var data = mappingHelper.MapPostsWithCategories(posts, categories);
                
                result.Success(data);

                logger.Information(
                    "END {MethodName} - Successfully retrieved {PostCount} posts for page {PageNumber} with page size {PageSize}",
                    methodName, data.MetaData.TotalItems, request.PageNumber, request.PageSize);
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