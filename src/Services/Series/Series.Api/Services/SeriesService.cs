using AutoMapper;
using Series.Api.Entities;
using Series.Api.Repositories.Interfaces;
using Series.Api.Services.Interfaces;
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

        try
        {
            if (string.IsNullOrEmpty(model.Slug))
            {
                model.Slug = Utils.ToUnSignString(model.Name);
            }

            var series = mapper.Map<SeriesBase>(model);
            await seriesRepository.CreateSeries(series);

            var data = mapper.Map<SeriesDto>(series);
            result.Success(data);
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

        try
        {
            var series = await seriesRepository.GetSeriesById(id);
            if (series == null)
            {
                result.Messages.Add("Series not found");
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var updateSeries = mapper.Map(model, series);
            await seriesRepository.UpdateSeries(updateSeries);

            var data = mapper.Map<SeriesDto>(updateSeries);
            result.Success(data);
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

        try
        {
            foreach (var id in ids)
            {
                var series = await seriesRepository.GetSeriesById(id);
                if (series == null)
                {
                    result.Messages.Add("Series not found");
                    result.Failure(StatusCodes.Status404NotFound, result.Messages);
                    return result;
                }

                await seriesRepository.DeleteSeries(series);
                result.Success(true);
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

        try
        {
            var series = await seriesRepository.GetSeries();
            if (series.IsNotNullOrEmpty())
            {
                var data = mapper.Map<IEnumerable<SeriesDto>>(series);
                result.Success(data);
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

        try
        {
            var series = await seriesRepository.GetSeriesById(id);
            if (series == null)
            {
                result.Messages.Add("Series not found");
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var data = mapper.Map<SeriesDto>(series);
            result.Success(data);
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

        try
        {
            var series = await seriesRepository.GetSeriesPaging(pageNumber, pageSize);
            var data = new PagedResponse<SeriesDto>()
            {
                Items = mapper.Map<List<SeriesDto>>(series.Items),
                MetaData = series.MetaData
            };

            result.Success(data);
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