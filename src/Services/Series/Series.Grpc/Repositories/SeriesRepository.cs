using Contracts.Domains.Repositories;
using Infrastructure.Domains.Repositories;
using Infrastructure.Paged;
using Microsoft.EntityFrameworkCore;
using Series.Grpc.Entities;
using Series.Grpc.Persistence;
using Series.Grpc.Repositories.Interfaces;
using Shared.Responses;

namespace Series.Grpc.Repositories;

public class SeriesRepository(SeriesContext dbContext, IUnitOfWork<SeriesContext> unitOfWork)
    : RepositoryCommandBase<SeriesBase, Guid, SeriesContext>(dbContext, unitOfWork), ISeriesRepository
{
    #region CRUD

    public async Task<SeriesBase?> GetSeriesById(Guid id) => await GetByIdAsync(id) ?? null;

    #endregion
}