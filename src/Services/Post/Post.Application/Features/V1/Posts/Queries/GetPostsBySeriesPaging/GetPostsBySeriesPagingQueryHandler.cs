using Contracts.Commons.Interfaces;
using Infrastructure.Paged;
using MediatR;
using Microsoft.AspNetCore.Http;
using Post.Domain.GrpcClients;
using Post.Domain.Repositories;
using Post.Domain.Services;
using Serilog;
using Shared.Constants;
using Shared.Dtos.Post;
using Shared.Helpers;
using Shared.Responses;
using Shared.Utilities;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsBySeriesPaging;

public class GetPostsBySeriesPagingQueryHandler(
    IPostRepository postRepository,
    ISeriesGrpcClient seriesGrpcClient,
    IPostInSeriesGrpcClient postInSeriesGrpcClient,
    IPostService postService,
    ICacheService cacheService,
    ILogger logger) : IRequestHandler<GetPostsBySeriesPagingQuery, ApiResult<PostsBySeriesDto>>
{
    public async Task<ApiResult<PostsBySeriesDto>> Handle(GetPostsBySeriesPagingQuery query,
        CancellationToken cancellationToken)
    {
        var result = new ApiResult<PostsBySeriesDto>();
        const string methodName = nameof(GetPostsBySeriesPagingQuery);

        try
        {
            logger.Information(
                "BEGIN {MethodName} - Retrieving posts by series {SeriesSlug} on page {PageNumber} with page size {PageSize}",
                methodName, query.SeriesSlug, query.Request.PageNumber, query.Request.PageSize);

            var cacheKey = CacheKeyHelper.Post.GetPostsBySeriesPagingKey(query.SeriesSlug, query.Request.PageNumber,
                query.Request.PageSize);
            var cached = await cacheService.GetAsync<PostsBySeriesDto>(cacheKey, cancellationToken);
            if (cached != null)
            {
                logger.Information(
                    "END {MethodName} - Successfully retrieved posts from cache for category slug {SeriesSlug} on page {PageNumber} with page size {PageSize}",
                    methodName, query.SeriesSlug, query.Request.PageNumber, query.Request.PageSize);
                result.Success(cached);
                return result;
            }

            var series = await seriesGrpcClient.GetSeriesBySlug(query.SeriesSlug);
            if (series == null)
            {
                logger.Warning("{MethodName} - Series not found with slug: {SeriesSlug}", methodName,
                    query.SeriesSlug);

                result.Messages.Add(ErrorMessagesConsts.Category.CategoryNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var postIds = await postInSeriesGrpcClient.GetPostIdsInSeries(series.Id);
            var postIdList = postIds.ToArray();

            var posts = await postRepository.GetPostsByIds(postIdList);
            var postList = posts.ToList();

            if (!postList.IsNotNullOrEmpty())
            {
                result.Messages.Add(ErrorMessagesConsts.Post.PostNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var enrichedPosts = await postService.EnrichPostsWithCategories(postList, cancellationToken);

            var items = PagedList<PostDto>.ToPagedList(enrichedPosts, query.Request.PageNumber, query.Request.PageSize,
                x => x.Id);

            var data = new PostsBySeriesDto()
            {
                Series = series,
                Posts = new PagedResponse<PostDto>()
                {
                    Items = items,
                    MetaData = items.GetMetaData()
                }
            };

            result.Success(data);
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