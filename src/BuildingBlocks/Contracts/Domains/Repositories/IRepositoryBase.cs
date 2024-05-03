using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Contracts.Domains.Repositories;

#region Query Base (Get)

public interface IRepositoryQueryBase<T, in TK> where T : EntityBase<TK>
{
    IQueryable<T> FindAll(bool trackChanges = false);
    
    IQueryable<T> FindAll(bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties);
    
    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false);

    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false,
        params Expression<Func<T, object>>[] includeProperties);

    Task<T?> GetByIdAsync(TK id);
    
    Task<T?> GetByIdAsync(TK id, params Expression<Func<T, object>>[] includeProperties);
}

public interface IRepositoryQueryBase<T, in TK, TContext> : IRepositoryQueryBase<T, TK> where T : EntityBase<TK>
    where TContext : DbContext;

#endregion

#region Command Base (Create-Update-Delete)

public interface IRepositoryCommandBase<T, TK> : IRepositoryQueryBase<T, TK>
    where T : EntityBase<TK>
{
    void Create(T entity);
    
    Task<TK> CreateAsync(T entity);
    
    IList<TK> CreateList(IEnumerable<T> entities);
    
    Task<IList<TK>> CreateListAsync(IEnumerable<T> entities);
    
    void Update(T entity);
    
    Task UpdateAsync(T entity);
    
    void UpdateList(IEnumerable<T> entities);
    
    Task UpdateListAsync(IEnumerable<T> entities);
    
    void Delete(T entity);
    
    Task DeleteAsync(T entity);
    
    void DeleteList(IEnumerable<T> entities);
    
    Task DeleteListAsync(IEnumerable<T> entities);
    
    Task<int> SaveChangesAsync();
    
    Task<IDbContextTransaction> BeginTransactionAsync();
    
    Task EndTransactionAsync();
    
    Task RollbackTransactionAsync();
}

public interface IRepositoryCommandBase<T, TK, TContext> : IRepositoryCommandBase<T, TK>
    where T : EntityBase<TK>
    where TContext : DbContext;

#endregion