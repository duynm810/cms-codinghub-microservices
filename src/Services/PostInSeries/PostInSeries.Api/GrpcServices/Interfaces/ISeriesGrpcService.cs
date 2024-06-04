using Shared.Dtos.Series;

namespace PostInSeries.Api.GrpcServices.Interfaces;

public interface ISeriesGrpcService
{
    Task<SeriesDto?> GetSeriesById(Guid id);

    Task<SeriesDto?> GetSeriesBySlug(string slug);
}