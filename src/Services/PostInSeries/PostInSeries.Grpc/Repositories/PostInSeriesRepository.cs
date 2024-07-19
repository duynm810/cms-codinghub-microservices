using Contracts.Domains.Repositories;
using Infrastructure.Domains.Repositories;
using PostInSeries.Grpc.Entities;
using PostInSeries.Grpc.Persistence;
using PostInSeries.Grpc.Repositories.Interfaces;

namespace PostInSeries.Grpc.Repositories;

public class PostInSeriesRepository(PostInSeriesContext dbContext, IUnitOfWork<PostInSeriesContext> unitOfWork)
    : RepositoryCommandBase<PostInSeriesBase, Guid, PostInSeriesContext>(dbContext, unitOfWork), IPostInSeriesRepository
{
    
}