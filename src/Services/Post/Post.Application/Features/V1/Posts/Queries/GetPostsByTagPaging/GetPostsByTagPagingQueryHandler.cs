using AutoMapper;
using Contracts.Commons.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Domain.GrpcClients;
using Post.Domain.Repositories;
using Serilog;
using Shared.Dtos.Post.Queries;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsByTagPaging;

public class GetPostsByTagPagingQueryHandler(
    IPostRepository postRepository,
    ITagGrpcClient tagGrpcClient,
    ICacheService cacheService,
    IMapper mapper,
    ILogger logger) : IRequestHandler<GetPostsByTagPagingQuery, ApiResult<PostsByTagDto>>
{
    public async Task<ApiResult<PostsByTagDto>> Handle(GetPostsByTagPagingQuery request,
        CancellationToken cancellationToken)
    {
        var result = new ApiResult<PostsByTagDto>();
        const string methodName = nameof(GetPostsByTagPagingQuery);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving posts by tag with Slug: {TagSlug} for page {PageNumber} with page size {PageSize}", methodName, request.TagSlug, request.PageNumber, request.PageSize);
            
            var tag = await tagGrpcClient.GetTagBySlug(request.TagSlug);
            if (tag != null)
            {
                
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