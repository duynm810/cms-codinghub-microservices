using Contracts.Domains.Repositories;
using PostInSeries.Grpc.Entities;

namespace PostInSeries.Grpc.Repositories.Interfaces;

public interface IPostInSeriesRepository : IRepositoryCommandBase<PostInSeriesBase, Guid>
{
}