using Shared.Dtos.PostInSeries;

namespace PostInSeries.Api.GrpcClients.Interfaces;

public interface IPostGrpcClient
{
    Task<IEnumerable<PostInSeriesDto>> GetPostsByIds(IEnumerable<Guid> ids);
}