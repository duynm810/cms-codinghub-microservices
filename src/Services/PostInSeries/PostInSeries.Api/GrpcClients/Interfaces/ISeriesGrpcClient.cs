using Shared.Dtos.Series;

namespace PostInSeries.Api.GrpcClients.Interfaces;

public interface ISeriesGrpcClient
{
    Task<SeriesDto?> GetSeriesById(Guid id);

    Task<SeriesDto?> GetSeriesBySlug(string slug);
}