using Contracts.Domains.Repositories;
using Infrastructure.Domains.Repositories;
using Microsoft.EntityFrameworkCore;
using PostInSeries.Grpc.Entities;
using PostInSeries.Grpc.Persistence;
using PostInSeries.Grpc.Repositories.Interfaces;

namespace PostInSeries.Grpc.Repositories;

public class PostInSeriesRepository(PostInSeriesContext dbContext, IUnitOfWork<PostInSeriesContext> unitOfWork)
    : RepositoryCommandBase<PostInSeriesBase, Guid, PostInSeriesContext>(dbContext, unitOfWork), IPostInSeriesRepository
{
    public async Task<List<Guid>?> GetPostIdsBySeriesId(Guid seriesId) =>
        await FindByCondition(x => x.SeriesId == seriesId)
            .Select(x => x.PostId)
            .ToListAsync();
}