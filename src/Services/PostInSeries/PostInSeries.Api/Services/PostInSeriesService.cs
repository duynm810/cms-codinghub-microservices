using Infrastructure.Paged;
using PostInSeries.Api.GrpcServices.Interfaces;
using PostInSeries.Api.Repositories.Interfaces;
using PostInSeries.Api.Services.Interfaces;
using Shared.Constants;
using Shared.Dtos.PostInSeries;
using Shared.Responses;
using Shared.Utilities;
using ILogger = Serilog.ILogger;

namespace PostInSeries.Api.Services;

public class PostInSeriesService(
    IPostInSeriesRepository postInSeriesRepository,
    IPostGrpcService postGrpcService,
    ISeriesGrpcService seriesGrpcService,
    ILogger logger) : IPostInSeriesService
{
    #region CRUD

    public async Task<ApiResult<bool>> CreatePostToSeries(Guid seriesId, Guid postId, int sortOrder)
    {
        var result = new ApiResult<bool>();
        const string methodName = nameof(CreatePostToSeries);

        try
        {
            logger.Information(
                "BEGIN {MethodName} - Creating post with ID: {PostId} to series with ID: {SeriesId} with sort order: {SortOrder}",
                methodName, postId, seriesId, sortOrder);

            await postInSeriesRepository.CreatePostToSeries(seriesId, postId, sortOrder);
            result.Success(true);

            logger.Information(
                "END {MethodName} - Successfully created post with ID: {PostId} to series with ID: {SeriesId}",
                methodName, postId, seriesId);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(CreatePostToSeries), e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<bool>> DeletePostToSeries(Guid seriesId, Guid postId)
    {
        var result = new ApiResult<bool>();
        const string methodName = nameof(DeletePostToSeries);

        try
        {
            logger.Information("BEGIN {MethodName} - Deleting post with ID: {PostId} from series with ID: {SeriesId}",
                methodName, postId, seriesId);

            await postInSeriesRepository.DeletePostToSeries(seriesId, postId);
            result.Success(true);

            logger.Information(
                "END {MethodName} - Successfully deleted post with ID: {PostId} from series with ID: {SeriesId}",
                methodName, postId, seriesId);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(DeletePostToSeries), e);
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
            logger.Information("BEGIN {MethodName} - Retrieving posts in series with ID: {SeriesId}", methodName,
                seriesId);

            var postIds = await postInSeriesRepository.GetPostIdsInSeries(seriesId);
            if (postIds == null)
            {
                logger.Warning("{MethodName} - Post IDs not found for series with ID: {SeriesId}", methodName,
                    seriesId);
                result.Messages.Add(ErrorMessagesConsts.PostInSeries.PostIdsNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var postList = postIds.ToList();
            if (postList.Count != 0)
            {
                var postInSeriesDtos = await postGrpcService.GetPostsByIds(postList);
                var data = postInSeriesDtos.ToList();
                result.Success(data);

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
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(GetPostsInSeries), e);
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
            logger.Information("BEGIN {MethodName} - Retrieving posts in series with Slug: {SeriesSlug}", methodName,
                seriesSlug);

            var series = await seriesGrpcService.GetSeriesBySlug(seriesSlug);
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
                    var postInSeriesDtos = await postGrpcService.GetPostsByIds(postList);
                    var data = postInSeriesDtos.ToList();
                    result.Success(data);

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
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(GetPostsInSeries), e);
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

            var postIds = await postInSeriesRepository.GetPostIdsInSeries(seriesId);
            if (postIds == null)
            {
                logger.Warning("{MethodName} - Post IDs not found for series with ID: {SeriesId}", methodName,
                    seriesId);
                result.Messages.Add(ErrorMessagesConsts.PostInSeries.PostIdsNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var postList = postIds.ToList();
            if (postList.Count != 0)
            {
                var posts = await postGrpcService.GetPostsByIds(postList);
                var items = PagedList<PostInSeriesDto>.ToPagedList(posts, pageNumber, pageSize, x => x.Id);

                var data = new PagedResponse<PostInSeriesDto>
                {
                    Items = items,
                    MetaData = items.GetMetaData()
                };

                result.Success(data);

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
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(GetPostsInSeriesPaging), e);
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

            var series = await seriesGrpcService.GetSeriesBySlug(seriesSlug);
            if (series != null)
            {
                var postIds = await postInSeriesRepository.GetPostIdsInSeries(series.Id);
                if (postIds == null)
                {
                    logger.Warning("{MethodName} - Post IDs not found for series with Slug: {SeriesSlug}", methodName, series.Slug);
                    result.Messages.Add(ErrorMessagesConsts.PostInSeries.PostIdsNotFound);
                    result.Failure(StatusCodes.Status404NotFound, result.Messages);
                    return result;
                }
                
                var postList = postIds.ToList();
                if (postList.Count != 0)
                {
                    var posts = await postGrpcService.GetPostsByIds(postList);
                    var items = PagedList<PostInSeriesDto>.ToPagedList(posts, pageNumber, pageSize, x => x.Id);

                    var data = new PagedResponse<PostInSeriesDto>
                    {
                        Items = items,
                        MetaData = items.GetMetaData()
                    };

                    result.Success(data);

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