using AutoMapper;
using Contracts.Commons.Interfaces;
using PostInSeries.Api.Entities;
using PostInSeries.Api.GrpcClients.Interfaces;
using PostInSeries.Api.Repositories.Interfaces;
using PostInSeries.Api.Services.Interfaces;
using Shared.Constants;
using Shared.Dtos.PostInSeries;
using Shared.Dtos.Series;
using Shared.Helpers;
using Shared.Requests.PostInSeries;
using Shared.Responses;
using Shared.Utilities;
using ILogger = Serilog.ILogger;

namespace PostInSeries.Api.Services;

public class PostInSeriesService(
    IPostInSeriesRepository postInSeriesRepository,
    ISeriesGrpcClient seriesGrpcClient,
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

    public async Task<ApiResult<bool>> DeletePostToSeries(Guid postId, Guid seriesId)
    {
        var result = new ApiResult<bool>();
        const string methodName = nameof(DeletePostToSeries);

        try
        {
            logger.Information("BEGIN {MethodName} - Deleting post with ID: {PostId} from series with ID: {SeriesId}",
                methodName, postId, seriesId);

            var postInSeries = await postInSeriesRepository.GetPostInSeries(postId, seriesId);
            if (postInSeries == null)
            {
                logger.Warning("{MethodName} - Post: {PostId} not found in series: {SeriesId}", methodName, postId, seriesId);
                result.Messages.Add(ErrorMessagesConsts.PostInSeries.PostNotFoundInSeries);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }
            
            await postInSeriesRepository.DeletePostToSeries(postInSeries);
            result.Success(true);

            // Xoá cache
            await cacheService.RemoveAsync(CacheKeyHelper.PostInSeries.GetAllPostInSeriesByIdKey(seriesId));

            logger.Information(
                "END {MethodName} - Successfully deleted post with ID: {PostId} from series with ID: {SeriesId}",
                methodName, postId, seriesId);
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

    public async Task<ApiResult<ManagePostInSeriesDto>> GetSeriesForPost(Guid postId)
    {
        var result = new ApiResult<ManagePostInSeriesDto>();
        const string methodName = nameof(GetSeriesForPost);

        try
        {
            logger.Information(
                "BEGIN {MethodName} - Retrieved manage series for post with ID: {PostId}",
                methodName, postId);
            
            var seriesIds = await postInSeriesRepository.GetSeriesIdsByPostId(postId);
            var currentSeries = new List<SeriesDto>();

            var seriesIdList = seriesIds.ToList();
            if (seriesIdList.Count != 0)
            {
                currentSeries = await seriesGrpcClient.GetSeriesByIds(seriesIdList);
            }
            
            var allSeries = await seriesGrpcClient.GetAllSeries();

            var data = new ManagePostInSeriesDto()
            {
                Series = allSeries,
                CurrentSeries = currentSeries
            };

            result.Success(data);
            
            logger.Information(
                "END {MethodName} - Successfully retrieved manage series for post with ID: {PostId}",
                methodName, postId);
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