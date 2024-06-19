using AutoMapper;
using Contracts.Commons.Interfaces;
using Infrastructure.Paged;
using PostInSeries.Api.Entities;
using PostInSeries.Api.GrpcClients.Interfaces;
using PostInSeries.Api.Repositories.Interfaces;
using PostInSeries.Api.Services.Interfaces;
using Shared.Constants;
using Shared.Dtos.Category;
using Shared.Dtos.PostInSeries;
using Shared.Helpers;
using Shared.Responses;
using Shared.Utilities;
using ILogger = Serilog.ILogger;

namespace PostInSeries.Api.Services;

public class PostInSeriesService(
    IPostInSeriesRepository postInSeriesRepository,
    IPostGrpcClient postGrpcClient,
    ISeriesGrpcClient seriesGrpcClient,
    ICategoryGrpcClient categoryGrpcClient,
    ICacheService cacheService,
    IMapper mapper,
    ILogger logger) : IPostInSeriesService
{
    #region CRUD

    public async Task<ApiResult<bool>> CreatePostToSeries(CreatePostInSeriesDto request)
    {
        var result = new ApiResult<bool>();
        const string methodName = nameof(CreatePostToSeries);

        try
        {
            logger.Information(
                "BEGIN {MethodName} - Creating post with ID: {PostId} to series with ID: {SeriesId} with sort order: {SortOrder}",
                methodName, request.PostId, request.SeriesId, request.SortOrder);

            var postInSeries = mapper.Map<PostInSeriesBase>(request);

            await postInSeriesRepository.CreatePostToSeries(postInSeries);
            result.Success(true);

            // Xoá cache
            await cacheService.RemoveAsync(CacheKeyHelper.PostInSeries.GetAllPostInSeriesByIdKey(request.SeriesId));

            logger.Information(
                "END {MethodName} - Successfully created post with ID: {PostId} to series with ID: {SeriesId}",
                methodName, request.PostId, request.SeriesId);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<bool>> DeletePostToSeries(DeletePostInSeriesDto request)
    {
        var result = new ApiResult<bool>();
        const string methodName = nameof(DeletePostToSeries);

        try
        {
            logger.Information("BEGIN {MethodName} - Deleting post with ID: {PostId} from series with ID: {SeriesId}",
                methodName, request.PostId, request.SeriesId);

            var postInSeries = mapper.Map<PostInSeriesBase>(request);

            await postInSeriesRepository.DeletePostToSeries(postInSeries);
            result.Success(true);
            
            // Xoá cache
            await cacheService.RemoveAsync(CacheKeyHelper.PostInSeries.GetAllPostInSeriesByIdKey(request.SeriesId));

            logger.Information(
                "END {MethodName} - Successfully deleted post with ID: {PostId} from series with ID: {SeriesId}",
                methodName, request.PostId, request.SeriesId);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    #endregion

    #region OTHERS

    public async Task<ApiResult<IEnumerable<PostInSeriesDto>>> GetPostsInSeries(Guid seriesId)
    {
        var result = new ApiResult<IEnumerable<PostInSeriesDto>>();
        const string methodName = nameof(GetPostsInSeries);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving posts in series with ID: {SeriesId}", methodName, seriesId);
            
            var cacheKey = CacheKeyHelper.PostInSeries.GetAllPostInSeriesByIdKey(seriesId);
            var cachedPostInSeries = await cacheService.GetAsync<IEnumerable<PostInSeriesDto>>(cacheKey);
            if (cachedPostInSeries != null)
            {
                result.Success(cachedPostInSeries);
                logger.Information("END {MethodName} - Successfully retrieved post in series with ID {SeriesId} from cache", methodName, seriesId);
                return result;
            }

            var postIds = await postInSeriesRepository.GetPostIdsInSeries(seriesId);
            if (postIds == null)
            {
                logger.Warning("{MethodName} - Post IDs not found for series with ID: {SeriesId}", methodName, seriesId);
                result.Messages.Add(ErrorMessagesConsts.PostInSeries.PostIdsNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var postList = postIds.ToList();
            if (postList.Count != 0)
            {
                var postInSeriesDtos = await postGrpcClient.GetPostsByIds(postList);
                var data = postInSeriesDtos.ToList();
                result.Success(data);
                
                // Save cache (Lưu cache)
                await cacheService.SetAsync(cacheKey, data);

                logger.Information(
                    "END {MethodName} - Successfully retrieved {PostCount} posts for series with ID: {SeriesId}",
                    methodName, data.Count, seriesId);
            }
            else
            {
                result.Messages.Add(ErrorMessagesConsts.PostInSeries.PostNotFoundInSeries);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
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

    public async Task<ApiResult<IEnumerable<PostInSeriesDto>>> GetPostsInSeriesBySlug(string seriesSlug)
    {
        var result = new ApiResult<IEnumerable<PostInSeriesDto>>();
        const string methodName = nameof(GetPostsInSeriesBySlug);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving posts in series with Slug: {SeriesSlug}", methodName, seriesSlug);

            var cacheKey = CacheKeyHelper.PostInSeries.GetPostInSeriesBySlugKey(seriesSlug);
            var cachedPostInSeries = await cacheService.GetAsync<IEnumerable<PostInSeriesDto>>(cacheKey);
            if (cachedPostInSeries != null)
            {
                result.Success(cachedPostInSeries);
                logger.Information("END {MethodName} - Successfully retrieved post in series with Slug {SeriesSlug} from cache", methodName, seriesSlug);
                return result;
            }
            
            var series = await seriesGrpcClient.GetSeriesBySlug(seriesSlug);
            if (series != null)
            {
                var postIds = await postInSeriesRepository.GetPostIdsInSeries(series.Id);
                if (postIds == null)
                {
                    logger.Warning("{MethodName} - Post IDs not found for series with Slug: {SeriesSlug}", methodName,
                        series.Slug);
                    result.Messages.Add(ErrorMessagesConsts.PostInSeries.PostIdsNotFound);
                    result.Failure(StatusCodes.Status404NotFound, result.Messages);
                    return result;
                }

                var postList = postIds.ToList();
                if (postList.Count != 0)
                {
                    var postInSeriesDtos = await postGrpcClient.GetPostsByIds(postList);
                    var data = postInSeriesDtos.ToList();
                    result.Success(data);
                    
                    // Save cache (Lưu cache)
                    await cacheService.SetAsync(cacheKey, data);

                    logger.Information(
                        "END {MethodName} - Successfully retrieved {PostCount} posts for series with Slug: {SeriesSlug}",
                        methodName, data.Count, series.Slug);
                }
                else
                {
                    result.Messages.Add(ErrorMessagesConsts.PostInSeries.PostNotFoundInSeries);
                    result.Failure(StatusCodes.Status404NotFound, result.Messages);
                }
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

    public async Task<ApiResult<PagedResponse<PostInSeriesDto>>> GetPostsInSeriesPaging(Guid seriesId, int pageNumber,
        int pageSize)
    {
        var result = new ApiResult<PagedResponse<PostInSeriesDto>>();
        const string methodName = nameof(GetPostsInSeriesPaging);

        try
        {
            logger.Information(
                "BEGIN {MethodName} - Retrieving posts in series with ID: {SeriesId} for page {PageNumber} with page size {PageSize}",
                methodName, seriesId, pageNumber, pageSize);
            
            var cacheKey = CacheKeyHelper.PostInSeries.GetPostInSeriesByIdPagingKey(seriesId, pageNumber, pageSize);
            var cachedPostInSeries = await cacheService.GetAsync<PagedResponse<PostInSeriesDto>>(cacheKey);
            if (cachedPostInSeries != null)
            {
                result.Success(cachedPostInSeries);
                logger.Information("END {MethodName} - Successfully retrieved post in series with ID {SeriesId} from cache", methodName, seriesId);
                return result;
            }

            var postIds = await postInSeriesRepository.GetPostIdsInSeries(seriesId);
            if (postIds == null)
            {
                logger.Warning("{MethodName} - Post IDs not found for series with ID: {SeriesId}", methodName, seriesId);
                result.Messages.Add(ErrorMessagesConsts.PostInSeries.PostIdsNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var postList = postIds.ToList();
            if (postList.Any())
            {
                var posts = await postGrpcClient.GetPostsByIds(postList);
                var items = PagedList<PostInSeriesDto>.ToPagedList(posts, pageNumber, pageSize, x => x.Id);

                var data = new PagedResponse<PostInSeriesDto>
                {
                    Items = items,
                    MetaData = items.GetMetaData()
                };

                result.Success(data);
                
                // Save cache (Lưu cache)
                await cacheService.SetAsync(cacheKey, data);

                logger.Information(
                    "END {MethodName} - Successfully retrieved {PostCount} posts for series with ID: {SeriesId} for page {PageNumber} with page size {PageSize}",
                    methodName, items.Count, seriesId, pageNumber, pageSize);
            }
            else
            {
                result.Messages.Add(ErrorMessagesConsts.PostInSeries.PostNotFoundInSeries);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
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

    public async Task<ApiResult<PagedResponse<PostInSeriesDto>>> GetPostsInSeriesBySlugPaging(string seriesSlug,
        int pageNumber, int pageSize)
    {
        var result = new ApiResult<PagedResponse<PostInSeriesDto>>();
        const string methodName = nameof(GetPostsInSeriesBySlugPaging);

        try
        {
            logger.Information(
                "BEGIN {MethodName} - Retrieving posts in series with Slug: {SeriesSlug} for page {PageNumber} with page size {PageSize}",
                methodName, seriesSlug, pageNumber, pageSize);
            
            var cacheKey = CacheKeyHelper.PostInSeries.GetPostInSeriesBySlugPagingKey(seriesSlug, pageNumber, pageSize);
            var cachedPostInSeries = await cacheService.GetAsync<PagedResponse<PostInSeriesDto>>(cacheKey);
            if (cachedPostInSeries != null)
            {
                result.Success(cachedPostInSeries);
                logger.Information("END {MethodName} - Successfully retrieved post in series with Slug {SeriesSlug} from cache", methodName, seriesSlug);
                return result;
            }

            var series = await seriesGrpcClient.GetSeriesBySlug(seriesSlug);
            if (series != null)
            {
                var postIds = await postInSeriesRepository.GetPostIdsInSeries(series.Id);
                if (postIds == null)
                {
                    logger.Warning("{MethodName} - Post IDs not found for series with Slug: {SeriesSlug}", methodName,
                        series.Slug);
                    result.Messages.Add(ErrorMessagesConsts.PostInSeries.PostIdsNotFound);
                    result.Failure(StatusCodes.Status404NotFound, result.Messages);
                    return result;
                }

                var postIdList = postIds.ToList();
                if (postIdList.Any())
                {
                    var posts = await postGrpcClient.GetPostsByIds(postIdList);

                    var postList = posts.ToList();

                    var categoryIds = postList.Select(p => p.CategoryId).Distinct().ToList();
                    var categories = await categoryGrpcClient.GetCategoriesByIds(categoryIds);
                    var categoryDictionary = categories.ToDictionary(c => c.Id, c => c);

                    foreach (var post in postList)
                    {
                        if (categoryDictionary.TryGetValue(post.CategoryId, out var category))
                        {
                            mapper.Map(category, post);
                        }
                    }

                    var items = PagedList<PostInSeriesDto>.ToPagedList(postList, pageNumber, pageSize, x => x.Id);

                    var data = new PagedResponse<PostInSeriesDto>
                    {
                        Items = items,
                        MetaData = items.GetMetaData()
                    };

                    result.Success(data);
                    
                    // Save cache (Lưu cache)
                    await cacheService.SetAsync(cacheKey, data);

                    logger.Information(
                        "END {MethodName} - Successfully retrieved {PostCount} posts for series with Slug: {SeriesSlug} for page {PageNumber} with page size {PageSize}",
                        methodName, items.Count, series.Slug, pageNumber, pageSize);
                }
                else
                {
                    result.Messages.Add(ErrorMessagesConsts.PostInSeries.PostNotFoundInSeries);
                    result.Failure(StatusCodes.Status404NotFound, result.Messages);
                }
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

    #endregion
}