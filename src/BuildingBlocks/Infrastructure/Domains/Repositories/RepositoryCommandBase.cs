using Contracts.Domains;
using Contracts.Domains.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Domains.Repositories;

public class RepositoryCommandBase<T, TK> : RepositoryQueryBase<T, TK> where T : EntityBase<TK>;

public class RepositoryCommandBase<T, TK, TContext>(TContext dbContext, IUnitOfWork<TContext> unitOfWork)
    : RepositoryQueryBase<T, TK, TContext>(dbContext), IRepositoryCommandBase<T, TK, TContext>
    where T : EntityBase<TK>
    where TContext : DbContext
{
    private readonly TContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    private readonly IUnitOfWork<TContext> _unitOfWork =
        unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    #region Async

    public async Task<TK> CreateAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
        await _dbContext.SaveChangesAsync(); //open without using UnitOfWork, close if using UnitOfWork
        return entity.Id;
    }

    public async Task<IList<TK>> CreateListAsync(IEnumerable<T> entities)
    {
        var entityBases = entities.ToList();
        await _dbContext.Set<T>().AddRangeAsync(entityBases);
        return entityBases.Select(x => x.Id).ToList();
    }

    public async Task UpdateAsync(T entity)
    {
        if (_dbContext.Entry(entity).State == EntityState.Unchanged)
            return;

        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateListAsync(IEnumerable<T> entities)
    {
        await _dbContext.Set<T>().AddRangeAsync(entities);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteListAsync(IEnumerable<T> entities)
    {
        _dbContext.Set<T>().RemoveRange(entities);
        await _dbContext.SaveChangesAsync();
    }

    #endregion

    #region Not Async

    public void Create(T entity) =>
        _dbContext.Set<T>().Add(entity);

    public IList<TK> CreateList(IEnumerable<T> entities)
    {
        var entityBases = entities.ToList();
        _dbContext.Set<T>().AddRange(entityBases);
        return entityBases.Select(x => x.Id).ToList();
    }

    public void Update(T entity)
    {
        if (_dbContext.Entry(entity).State == EntityState.Unchanged)
        {
            return;
        }

        _dbContext.Entry(entity).CurrentValues.SetValues(entity);
    }

    public void UpdateList(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
        {
            Update(entity);
        }
    }

    public void Delete(T entity) => _dbContext.Set<T>().Remove(entity);

    public void DeleteList(IEnumerable<T> entities) => _dbContext.Set<T>().RemoveRange(entities);

    #endregion

    #region UOF (Unit Of Work)

    public Task<IDbContextTransaction> BeginTransactionAsync() => _dbContext.Database.BeginTransactionAsync();

    public async Task EndTransactionAsync()
    {
        await SaveChangesAsync();
        await _dbContext.Database.CommitTransactionAsync();
    }

    public Task RollbackTransactionAsync() => _dbContext.Database.RollbackTransactionAsync();

    public IExecutionStrategy CreateExecutionStrategy() => _dbContext.Database.CreateExecutionStrategy();

    public Task<int> SaveChangesAsync() => _unitOfWork.CommitAsync();

    #endregion
}