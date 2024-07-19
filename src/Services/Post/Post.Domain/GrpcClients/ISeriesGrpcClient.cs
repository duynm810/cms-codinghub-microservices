using Shared.Dtos.Series;

namespace Post.Domain.GrpcClients;

public interface ISeriesGrpcClient
{
    Task<SeriesDto?> GetSeriesBySlug(string slug);
}