using Contracts.Commons.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Domain.Repositories;
using Serilog;
using Shared.Dtos.Post;
using Shared.Helpers;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsBySeriesPaging;

public class GetPostsBySeriesPagingQueryHandler(IPostRepository postRepository, ICacheService cacheService, ILogger logger) : IRequestHandler<GetPostsBySeriesPagingQuery, ApiResult<PostsBySeriesDto>>
{
    public async Task<ApiResult<PostsBySeriesDto>> Handle(GetPostsBySeriesPagingQuery query, CancellationToken cancellationToken)
    {
        var result = new ApiResult<PostsBySeriesDto>();
        const string methodName = nameof(GetPostsBySeriesPagingQuery);
        
        try
        {
            logger.Information(
                "BEGIN {MethodName} - Retrieving posts by series {CategorySlug} on page {PageNumber} with page size {PageSize}",
                methodName, query.SeriesSlug, query.Request.PageNumber, query.Request.PageSize);
            
            var cacheKey = CacheKeyHelper.Post.GetPostsBySeriesPagingKey(query.SeriesSlug, query.Request.PageNumber, query.Request.PageSize);
            var cached = await cacheService.GetAsync<PostsBySeriesDto>(cacheKey, cancellationToken);
            if (cached != null)
            {
                logger.Information(
                    "END {MethodName} - Successfully retrieved posts from cache for category slug {CategorySlug} on page {PageNumber} with page size {PageSize}",
                    methodName, query.SeriesSlug, query.Request.PageNumber, query.Request.PageSize);
                result.Success(cached);
                return result;
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