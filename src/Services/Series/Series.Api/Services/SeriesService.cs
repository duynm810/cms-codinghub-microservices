using AutoMapper;
using Contracts.Commons.Interfaces;
using Series.Api.Entities;
using Series.Api.Repositories.Interfaces;
using Series.Api.Services.Interfaces;
using Shared.Constants;
using Shared.Dtos.Series;
using Shared.Helpers;
using Shared.Responses;
using Shared.Settings;
using Shared.Utilities;
using ILogger = Serilog.ILogger;

namespace Series.Api.Services;

public class SeriesService(
    ISeriesRepository seriesRepository,
    ICacheService cacheService,
    DisplaySettings displaySettings,
    IMapper mapper,
    ILogger logger) : ISeriesService
{
    #region CRUD

    public async Task<ApiResult<SeriesDto>> CreateSeries(CreateSeriesDto request)
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

            // Xóa cache danh sách series khi tạo mới
            await cacheService.RemoveAsync(CacheKeyHelper.Series.GetAllSeriesKey());

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

    public async Task<ApiResult<SeriesDto>> UpdateSeries(Guid id, UpdateSeriesDto request)
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

            // Delete series list cache when updating (Xóa cache danh sách series khi cập nhật)
            await cacheService.RemoveAsync(CacheKeyHelper.Series.GetAllSeriesKey());
            await cacheService.RemoveAsync(CacheKeyHelper.Series.GetSeriesByIdKey(id));
            await cacheService.RemoveAsync(CacheKeyHelper.Series.GetSeriesBySlugKey(series.Slug));

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

                await seriesRepository.DeleteSeries(series);

                // Delete series cache when delete (Xóa cache series khi xoá dữ liệu)
                await cacheService.RemoveAsync(CacheKeyHelper.Series.GetSeriesByIdKey(id));
                await cacheService.RemoveAsync(CacheKeyHelper.Series.GetSeriesBySlugKey(series.Slug));
            }

            // Delete series list cache when deleting (Xóa cache danh sách series khi xóa dữ liệu)
            await cacheService.RemoveAsync(CacheKeyHelper.Series.GetAllSeriesKey());

            result.Success(true);

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

            // Kiểm tra cache
            var cacheKey = CacheKeyHelper.Series.GetAllSeriesKey();
            var cachedSeries = await cacheService.GetAsync<IEnumerable<SeriesDto>>(cacheKey);
            if (cachedSeries != null)
            {
                result.Success(cachedSeries);
                logger.Information("END {MethodName} - Successfully retrieved series from cache", methodName);
                return result;
            }

            var count = displaySettings.Config.GetValueOrDefault(DisplaySettingsConsts.Series.TopSeries, 0);

            var series = await seriesRepository.GetSeries(count);
            if (series.IsNotNullOrEmpty())
            {
                var data = mapper.Map<List<SeriesDto>>(series);
                result.Success(data);

                // Lưu cache
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

            // Kiểm tra cache
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

            // Lưu cache
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

    public async Task<ApiResult<PagedResponse<SeriesDto>>> GetSeriesPaging(int pageNumber, int pageSize)
    {
        var result = new ApiResult<PagedResponse<SeriesDto>>();
        const string methodName = nameof(GetSeriesPaging);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving series for page {PageNumber} with page size {PageSize}",
                methodName, pageNumber, pageSize);

            // Kiểm tra cache
            var cacheKey = CacheKeyHelper.Series.GetSeriesPagingKey(pageNumber, pageSize);
            var cachedSeries = await cacheService.GetAsync<PagedResponse<SeriesDto>>(cacheKey);
            if (cachedSeries != null)
            {
                result.Success(cachedSeries);
                logger.Information("END {MethodName} - Successfully retrieved series for page {PageNumber} from cache",
                    methodName, pageNumber);
                return result;
            }

            var series = await seriesRepository.GetSeriesPaging(pageNumber, pageSize);
            var data = new PagedResponse<SeriesDto>()
            {
                Items = mapper.Map<List<SeriesDto>>(series.Items),
                MetaData = series.MetaData
            };

            result.Success(data);

            // Lưu cache
            await cacheService.SetAsync(cacheKey, data);

            logger.Information(
                "END {MethodName} - Successfully retrieved {SeriesCount} series for page {PageNumber} with page size {PageSize}",
                methodName, data.Items.Count, pageNumber, pageSize);
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

            // Kiểm tra cache
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

            // Lưu cache
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