using AutoMapper;
using Contracts.Commons.Interfaces;
using PostInSeries.Api.Entities;
using PostInSeries.Api.Repositories.Interfaces;
using PostInSeries.Api.Services.Interfaces;
using Shared.Dtos.PostInSeries;
using Shared.Helpers;
using Shared.Requests.PostInSeries;
using Shared.Responses;
using Shared.Utilities;
using ILogger = Serilog.ILogger;

namespace PostInSeries.Api.Services;

public class PostInSeriesService(
    IPostInSeriesRepository postInSeriesRepository,
    ICacheService cacheService,
    IMapper mapper,
    ILogger logger) : IPostInSeriesService
{
    #region CRUD

    public async Task<ApiResult<bool>> CreatePostToSeries(CreatePostInSeriesRequest request)
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

    public async Task<ApiResult<bool>> DeletePostToSeries(DeletePostInSeriesRequest request)
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
}