using AutoMapper;
using Contracts.Commons.Interfaces;
using Series.Api.Entities;
using Series.Api.Repositories.Interfaces;
using Series.Api.Services.Interfaces;
using Shared.Constants;
using Shared.Dtos.Series;
using Shared.Helpers;
using Shared.Requests.Series;
using Shared.Responses;
using Shared.Settings;
using Shared.Utilities;
using ILogger = Serilog.ILogger;

namespace Series.Api.Services;

public class SeriesService(
    ISeriesRepository seriesRepository,
    ICacheService cacheService,
    IMapper mapper,
    ILogger logger) : ISeriesService
{
    #region CRUD

    public async Task<ApiResult<SeriesDto>> CreateSeries(CreateSeriesRequest request)
    {
        var result = new ApiResult<SeriesDto>();
        const string methodName = nameof(CreateSeries);

        try
        {
            logger.Information("BEGIN {MethodName} - Creating series with name: {SeriesName}", methodName,
                request.Title);

            if (string.IsNullOrEmpty(request.Slug))
            {
                request.Slug = Utils.ToUnSignString(request.Title);
            }

            var series = mapper.Map<SeriesBase>(request);
            await seriesRepository.CreateSeries(series);

            var data = mapper.Map<SeriesDto>(series);
            result.Success(data);
            
            TaskHelper.RunFireAndForget(async () =>
            {
                var cacheKeys = new List<string>
                {
                    CacheKeyHelper.Series.GetAllSeriesKey(),
                    CacheKeyHelper.SeriesGrpc.GetAllSeriesKey()
                };

                await cacheService.RemoveMultipleAsync(cacheKeys);
            }, e =>
            {
                logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            });

            logger.Information("END {MethodName} - Series created successfully with ID {SeriesId}", methodName,
                data.Id);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<SeriesDto>> UpdateSeries(Guid id, UpdateSeriesRequest request)
    {
        var result = new ApiResult<SeriesDto>();
        const string methodName = nameof(UpdateSeries);

        try
        {
            logger.Information("BEGIN {MethodName} - Updating series with ID: {SeriesId}", methodName, id);

            var series = await seriesRepository.GetSeriesById(id);
            if (series == null)
            {
                logger.Warning("{MethodName} - Series not found with ID: {SeriesId}", methodName, id);
                result.Messages.Add(ErrorMessagesConsts.Series.SeriesNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var updateSeries = mapper.Map(request, series);
            await seriesRepository.UpdateSeries(updateSeries);

            var data = mapper.Map<SeriesDto>(updateSeries);
            result.Success(data);
            
            TaskHelper.RunFireAndForget(async () =>
            {
                var cacheKeys = new List<string>
                {
                    CacheKeyHelper.Series.GetAllSeriesKey(),
                    CacheKeyHelper.Series.GetSeriesByIdKey(id),
                    CacheKeyHelper.Series.GetSeriesBySlugKey(series.Slug),
                    CacheKeyHelper.SeriesGrpc.GetAllSeriesKey()
                };

                await cacheService.RemoveMultipleAsync(cacheKeys);
            }, e =>
            {
                logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            });

            logger.Information("END {MethodName} - Series with ID {SeriesId} updated successfully", methodName, id);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<bool>> DeleteSeries(List<Guid> ids)
    {
        var result = new ApiResult<bool>();
        const string methodName = nameof(DeleteSeries);

        try
        {
            logger.Information("BEGIN {MethodName} - Deleting series with IDs: {SeriesIds}", methodName,
                string.Join(", ", ids));

            var tasks = new List<Task>();
            
            foreach (var id in ids)
            {
                var series = await seriesRepository.GetSeriesById(id);
                if (series == null)
                {
                    logger.Warning("{MethodName} - Series not found with ID: {SeriesId}", methodName, id);
                    result.Messages.Add(ErrorMessagesConsts.Series.SeriesNotFound);
                    result.Failure(StatusCodes.Status404NotFound, result.Messages);
                    return result;
                }

                tasks.Add(seriesRepository.DeleteSeries(series));

                // Add cache removal tasks to the list (Thêm tác vụ xóa bộ nhớ cache vào danh sách)
                tasks.Add(cacheService.RemoveAsync(CacheKeyHelper.Series.GetSeriesByIdKey(id)));
                tasks.Add(cacheService.RemoveAsync(CacheKeyHelper.Series.GetSeriesBySlugKey(series.Slug)));
            }

            // Execute all tasks in parallel
            await Task.WhenAll(tasks);
            
            result.Success(true);
            
            TaskHelper.RunFireAndForget(async () =>
            {
                var cacheKeys = new List<string>
                {
                    CacheKeyHelper.Series.GetAllSeriesKey(),
                    CacheKeyHelper.SeriesGrpc.GetAllSeriesKey()
                };

                await cacheService.RemoveMultipleAsync(cacheKeys);
            }, e =>
            {
                logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            });

            logger.Information("END {MethodName} - Series with IDs {SeriesIds} deleted successfully", methodName,
                string.Join(", ", ids));
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<IEnumerable<SeriesDto>>> GetSeries()
    {
        var result = new ApiResult<IEnumerable<SeriesDto>>();
        const string methodName = nameof(GetSeries);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving all series", methodName);

            var cacheKey = CacheKeyHelper.Series.GetAllSeriesKey();
            var cachedSeries = await cacheService.GetAsync<IEnumerable<SeriesDto>>(cacheKey);
            if (cachedSeries != null)
            {
                result.Success(cachedSeries);
                logger.Information("END {MethodName} - Successfully retrieved series from cache", methodName);
                return result;
            }

            var series = await seriesRepository.GetSeries();
            if (series.IsNotNullOrEmpty())
            {
                var data = mapper.Map<List<SeriesDto>>(series);
                result.Success(data);

                // Save cache (Lưu cache)
                await cacheService.SetAsync(cacheKey, data);

                logger.Information("END {MethodName} - Successfully retrieved {SeriesCount} series", methodName,
                    data.Count);
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

    public async Task<ApiResult<SeriesDto>> GetSeriesById(Guid id)
    {
        var result = new ApiResult<SeriesDto>();
        const string methodName = nameof(GetSeriesById);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving series with ID: {SeriesId}", methodName, id);

            var cacheKey = CacheKeyHelper.Series.GetSeriesByIdKey(id);
            var cachedSeries = await cacheService.GetAsync<SeriesDto>(cacheKey);
            if (cachedSeries != null)
            {
                result.Success(cachedSeries);
                logger.Information("END {MethodName} - Successfully retrieved series with ID {SeriesId} from cache",
                    methodName, id);
                return result;
            }

            var series = await seriesRepository.GetSeriesById(id);
            if (series == null)
            {
                result.Messages.Add(ErrorMessagesConsts.Series.SeriesNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var data = mapper.Map<SeriesDto>(series);
            result.Success(data);

            // Save cache (Lưu cache)
            await cacheService.SetAsync(cacheKey, data);

            logger.Information("END {MethodName} - Successfully retrieved series with ID {SeriesId}", methodName, id);
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

    public async Task<ApiResult<PagedResponse<SeriesDto>>> GetSeriesPaging(GetSeriesRequest request)
    {
        var result = new ApiResult<PagedResponse<SeriesDto>>();
        const string methodName = nameof(GetSeriesPaging);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving series for page {PageNumber} with page size {PageSize}",
                methodName, request.PageNumber, request.PageSize);

            var cacheKey = CacheKeyHelper.Series.GetSeriesPagingKey(request.PageNumber, request.PageSize);
            var cachedSeries = await cacheService.GetAsync<PagedResponse<SeriesDto>>(cacheKey);
            if (cachedSeries != null)
            {
                result.Success(cachedSeries);
                logger.Information("END {MethodName} - Successfully retrieved series for page {PageNumber} from cache",
                    methodName, request.PageNumber);
                return result;
            }

            var series = await seriesRepository.GetSeriesPaging(request);
            var data = new PagedResponse<SeriesDto>()
            {
                Items = mapper.Map<List<SeriesDto>>(series.Items),
                MetaData = series.MetaData
            };

            result.Success(data);

            // Save cache (Lưu cache)
            await cacheService.SetAsync(cacheKey, data);

            logger.Information(
                "END {MethodName} - Successfully retrieved {SeriesCount} series for page {PageNumber} with page size {PageSize}",
                methodName, data.Items.Count, request.PageNumber, request.PageSize);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<SeriesDto>> GetSeriesBySlug(string slug)
    {
        var result = new ApiResult<SeriesDto>();
        const string methodName = nameof(GetSeriesBySlug);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving series with Slug: {SeriesSlug}", methodName, slug);

            var cacheKey = CacheKeyHelper.Series.GetSeriesBySlugKey(slug);
            var cachedSeries = await cacheService.GetAsync<SeriesDto>(cacheKey);
            if (cachedSeries != null)
            {
                result.Success(cachedSeries);
                logger.Information("END {MethodName} - Successfully retrieved series with slug {SeriesSlug} from cache",
                    methodName, slug);
                return result;
            }

            var series = await seriesRepository.GetSeriesBySlug(slug);
            if (series == null)
            {
                result.Messages.Add(ErrorMessagesConsts.Series.SeriesNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var data = mapper.Map<SeriesDto>(series);
            result.Success(data);

            // Save cache (Lưu cache)
            await cacheService.SetAsync(cacheKey, data);

            logger.Information("END {MethodName} - Successfully retrieved series Slug ID {SeriesSlug}", methodName,
                slug);
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