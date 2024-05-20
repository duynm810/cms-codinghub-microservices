namespace Contracts.Domains.Repositories;

public interface IDapperRepositoryQueryBase<T, TK> where T : EntityBase<TK>
{
    Task<IReadOnlyList<TModel>> QueryAsync<TModel>(string sql, object? parameters = null);

    Task<T?> QueryFirstOrDefaultAsync(string sql, object? parameters = null);

    Task<T?> QuerySingleAsync(string sql, object? parameters = null);
}

public interface IDapperRepositoryCommandBase<T, TK> : IDapperRepositoryQueryBase<T, TK> where T : EntityBase<TK>
{
    Task<int> ExecuteAsync(string sql, object? parameters = null);
}