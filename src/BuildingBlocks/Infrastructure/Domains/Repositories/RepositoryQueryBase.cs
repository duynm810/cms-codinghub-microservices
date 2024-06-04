using System.Data;
using System.Linq.Expressions;
using Contracts.Domains;
using Contracts.Domains.Repositories;
using Dapper;
using Infrastructure.Exceptions;
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

    public async Task<IReadOnlyList<TModel>> QueryAsync<TModel>(string sql, object? param,
        CommandType? commandType = CommandType.StoredProcedure, IDbTransaction? transaction = null,
        int? commandTimeout = 30)
        where TModel : EntityBase<TK>
    {
        var existingConnectionState = Connection.State;
        if (existingConnectionState != ConnectionState.Open)
            Connection.Open();

        try
        {
            return (await Connection.QueryAsync<TModel>(sql, param,
                transaction, 30, commandType)).AsList();
        }
        finally
        {
            if (existingConnectionState != ConnectionState.Open)
                Connection.Close();
        }
    }

    public async Task<TModel> QueryFirstOrDefaultAsync<TModel>(string sql, object? param,
        CommandType? commandType = CommandType.StoredProcedure, IDbTransaction? transaction = null,
        int? commandTimeout = 30)
        where TModel : EntityBase<TK>
    {
        var existingConnectionState = Connection.State;
        if (existingConnectionState != ConnectionState.Open)
            Connection.Open();

        try
        {
            var entity =
                await Connection.QueryFirstOrDefaultAsync<TModel>(sql, param, transaction, commandTimeout, commandType);
            if (entity == null) throw new EntityNotFoundException();
            return entity;
        }
        finally
        {
            if (existingConnectionState != ConnectionState.Open)
                Connection.Close();
        }
    }

    public async Task<TModel> QuerySingleAsync<TModel>(string sql, object? param,
        CommandType? commandType = CommandType.StoredProcedure, IDbTransaction? transaction = null,
        int? commandTimeout = 30)
        where TModel : EntityBase<TK>
    {
        var existingConnectionState = Connection.State;
        if (existingConnectionState != ConnectionState.Open)
            Connection.Open();

        try
        {
            return await Connection.QuerySingleAsync<TModel>(sql, param, transaction, commandTimeout, commandType);
        }
        finally
        {
            if (existingConnectionState != ConnectionState.Open)
                Connection.Close();
        }
    }
}