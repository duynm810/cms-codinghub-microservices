namespace Post.Domain.GrpcClients;

public interface IPostInSeriesGrpcClient
{
    Task<IEnumerable<Guid>> GetPostIdsInSeries(Guid seriesId);
}