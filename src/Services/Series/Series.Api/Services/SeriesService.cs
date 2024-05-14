using AutoMapper;
using Series.Api.Entities;
using Series.Api.Repositories.Interfaces;
using Series.Api.Services.Interfaces;
using Shared.Constants;
using Shared.Dtos.Series;
using Shared.Responses;
using Shared.Utilities;
using ILogger = Serilog.ILogger;

namespace Series.Api.Services;

public class SeriesService(ISeriesRepository seriesRepository, IMapper mapper, ILogger logger) : ISeriesService
{
    #region CRUD

    public async Task<ApiResult<SeriesDto>> CreateSeries(CreateSeriesDto model)
    {
        var result = new ApiResult<SeriesDto>();
        const string methodName = nameof(CreateSeries);

        try
        {
            logger.Information("BEGIN {MethodName} - Creating series with name: {SeriesName}", methodName, model.Name);

            if (string.IsNullOrEmpty(model.Slug))
            {
                model.Slug = Utils.ToUnSignString(model.Name);
            }

            var series = mapper.Map<SeriesBase>(model);
            await seriesRepository.CreateSeries(series);

            var data = mapper.Map<SeriesDto>(series);
            result.Success(data);

            logger.Information("END {MethodName} - Series created successfully with ID {SeriesId}", methodName,
                data.Id);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(CreateSeries), e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<SeriesDto>> UpdateSeries(Guid id, UpdateSeriesDto model)
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
                result.Messages.Add(ErrorMessageConsts.Series.SeriesNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var updateSeries = mapper.Map(model, series);
            await seriesRepository.UpdateSeries(updateSeries);

            var data = mapper.Map<SeriesDto>(updateSeries);
            result.Success(data);

            logger.Information("END {MethodName} - Series with ID {SeriesId} updated successfully", methodName, id);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(UpdateSeries), e);
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
            logger.Information("BEGIN {MethodName} - Deleting series with IDs: {SeriesIds}", methodName, string.Join(", ", ids));

            foreach (var id in ids)
            {
                var series = await seriesRepository.GetSeriesById(id);
                if (series == null)
                {
                    logger.Warning("{MethodName} - Series not found with ID: {SeriesId}", methodName, id);
                    result.Messages.Add(ErrorMessageConsts.Series.SeriesNotFound);
                    result.Failure(StatusCodes.Status404NotFound, result.Messages);
                    return result;
                }

                await seriesRepository.DeleteSeries(series);
                result.Success(true);
                
                logger.Information("END {MethodName} - Series with IDs {SeriesIds} deleted successfully", methodName, string.Join(", ", ids));
            }
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(DeleteSeries), e);
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

            var series = await seriesRepository.GetSeries();
            if (series.IsNotNullOrEmpty())
            {
                var data = mapper.Map<List<SeriesDto>>(series);
                result.Success(data);
                
                logger.Information("END {MethodName} - Successfully retrieved {SeriesCount} series", methodName, data.Count);
            }
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(GetSeries), e);
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

            var series = await seriesRepository.GetSeriesById(id);
            if (series == null)
            {
                result.Messages.Add(ErrorMessageConsts.Series.SeriesNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var data = mapper.Map<SeriesDto>(series);
            result.Success(data);
            
            logger.Information("END {MethodName} - Successfully retrieved series with ID {SeriesId}", methodName, id);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(GetSeriesById), e);
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
            logger.Information("BEGIN {MethodName} - Retrieving series for page {PageNumber} with page size {PageSize}", methodName, pageNumber, pageSize);

            var series = await seriesRepository.GetSeriesPaging(pageNumber, pageSize);
            var data = new PagedResponse<SeriesDto>()
            {
                Items = mapper.Map<List<SeriesDto>>(series.Items),
                MetaData = series.MetaData
            };

            result.Success(data);
            
            logger.Information("END {MethodName} - Successfully retrieved {SeriesCount} series for page {PageNumber} with page size {PageSize}", methodName, data.Items.Count, pageNumber, pageSize);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(GetSeriesPaging), e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    #endregion
}