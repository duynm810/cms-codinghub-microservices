using System.Data;
using System.Linq.Expressions;
using Contracts.Domains;
using Contracts.Domains.Repositories;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Domains.Repositories;

public class RepositoryQueryBase<T, TK> where T : EntityBase<TK>;

public class RepositoryQueryBase<T, TK, TContext>(TContext dbContext)
    : RepositoryQueryBase<T, TK>, IRepositoryQueryBase<T, TK, TContext>
    where T : EntityBase<TK>
    where TContext : DbContext
{
    private readonly TContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    private IDbConnection Connection => _dbContext.Database.GetDbConnection();
    
    public IQueryable<T> FindAll(bool trackChanges = false)
    {
        return !trackChanges
            ? _dbContext.Set<T>().AsNoTracking()
            : _dbContext.Set<T>();
    }

    public IQueryable<T> FindAll(bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
    {
        var items = FindAll(trackChanges);
        items = includeProperties.Aggregate(items, (current, includeProperty) => current.Include(includeProperty));
        return items;
    }

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false)
    {
        return !trackChanges
            ? _dbContext.Set<T>().Where(expression).AsNoTracking()
            : _dbContext.Set<T>().Where(expression);
    }

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false,
        params Expression<Func<T, object>>[] includeProperties)
    {
        var items = FindByCondition(expression, trackChanges);
        items = includeProperties.Aggregate(items, (current, includeProperty) => current.Include(includeProperty));
        return items;
    }

    public async Task<T?> GetByIdAsync(TK id)
    {
        return await FindByCondition(x => x.Id != null && x.Id.Equals(id))
            .FirstOrDefaultAsync();
    }

    public async Task<T?> GetByIdAsync(TK id, params Expression<Func<T, object>>[] includeProperties)
    {
        return await FindByCondition(x => x.Id != null && x.Id.Equals(id), false, includeProperties)
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<TModel>> QueryAsync<TModel>(string sql, object? parameters = null)
    {
        var existingConnectionState = Connection.State;
        if (existingConnectionState != ConnectionState.Open)
            Connection.Open();

        try
        {
            return (await Connection.QueryAsync<TModel>(sql, parameters)).AsList();
        }
        finally
        {
            if (existingConnectionState != ConnectionState.Open)
                Connection.Close();
        }
    }

    public async Task<T?> QueryFirstOrDefaultAsync(string sql, object? parameters = null)
    {
        var existingConnectionState = Connection.State;
        if (existingConnectionState != ConnectionState.Open)
            Connection.Open();

        try
        {
            return await Connection.QueryFirstOrDefaultAsync<T>(sql, parameters);
        }
        finally
        {
            if (existingConnectionState != ConnectionState.Open)
                Connection.Close();
        }
    }
}