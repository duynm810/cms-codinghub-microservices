using Contracts.Domains.Repositories;
using Infrastructure.Domains.Repositories;
using Microsoft.EntityFrameworkCore;
using PostInSeries.Api.Entities;
using PostInSeries.Api.Persistence;
using PostInSeries.Api.Repositories.Interfaces;

namespace PostInSeries.Api.Repositories;

public class PostInSeriesRepository(PostInSeriesContext dbContext, IUnitOfWork<PostInSeriesContext> unitOfWork)
    : RepositoryCommandBase<PostInSeriesBase, Guid, PostInSeriesContext>(dbContext, unitOfWork), IPostInSeriesRepository
{
    public async Task CreatePostToSeries(PostInSeriesBase postInSeriesBase) => await CreateAsync(postInSeriesBase);

    public async Task CreatePostsToSeries(IEnumerable<PostInSeriesBase> postInSeriesList)
    {
        await CreateListAsync(postInSeriesList);
    }

    public async Task DeletePostToSeries(PostInSeriesBase postInSeriesBase) => await DeleteAsync(postInSeriesBase);
    
    public async Task DeletePostToSeries(Guid postId)
    {
        await FindByCondition(x => x.PostId == postId).ExecuteDeleteAsync();
    }

    public async Task<PostInSeriesBase?> GetPostInSeries(Guid postId, Guid seriesId) =>
        await FindByCondition(x => x.PostId == postId && x.SeriesId == seriesId)
            .FirstOrDefaultAsync();
    
    public async Task<IEnumerable<Guid>?> GetPostIdsBySeriesId(Guid seriesId) =>
        await FindByCondition(x => x.SeriesId == seriesId)
            .Select(x => x.PostId)
            .ToListAsync();

    public async Task<IEnumerable<Guid>> GetSeriesIdsByPostId(Guid postId)
    {
        return await FindByCondition(x => x.PostId == postId)
            .Select(x => x.SeriesId)
            .ToListAsync();
    }
}