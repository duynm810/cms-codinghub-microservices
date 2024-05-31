using System.Text;
using Contracts.Commons.Interfaces;
using Contracts.Domains.Repositories;
using Infrastructure.Domains.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using PostInSeries.Api.Entities;
using PostInSeries.Api.Persistence;
using PostInSeries.Api.Repositories.Interfaces;

namespace PostInSeries.Api.Repositories;

public class PostInSeriesRepository(PostInSeriesContext dbContext, IUnitOfWork<PostInSeriesContext> unitOfWork)
    : RepositoryCommandBase<PostInSeriesBase, Guid, PostInSeriesContext>(dbContext, unitOfWork), IPostInSeriesRepository
{
    public async Task CreatePostToSeries(PostInSeriesBase postInSeriesBase) => await CreateAsync(postInSeriesBase);

    public async Task DeletePostToSeries(PostInSeriesBase postInSeriesBase) => await DeleteAsync(postInSeriesBase);

    public async Task<IEnumerable<Guid>?> GetPostIdsInSeries(Guid seriesId) =>
        await FindByCondition(x => x.SeriesId == seriesId).Select(x => x.SeriesId).ToListAsync();
}