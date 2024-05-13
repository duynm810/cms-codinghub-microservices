using Shared.Dtos.PostInSeries;

namespace PostInSeries.Api.GrpcServices.Interfaces;

public interface IPostGrpcService
{
    Task<IEnumerable<PostInSeriesDto>> GetPostsByIds(IEnumerable<Guid> ids);
}