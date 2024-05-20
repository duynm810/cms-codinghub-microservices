using System.Data;
using Contracts.Domains;
using Contracts.Domains.Repositories;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Domains.Repositories;

public class DapperRepositoryBase<T, TK>(DbContext dbContext) : IDapperRepositoryCommandBase<T, TK>
    where T : EntityBase<TK>
{
    private IDbConnection Connection => dbContext.Database.GetDbConnection();

    public async Task<IReadOnlyList<T>> QueryAsync(string sql, object? parameters = null)
    {
        var existingConnectionState = Connection.State;
        if (existingConnectionState != ConnectionState.Open)
            Connection.Open(); // Đảm bảo kết nối được mở

        try
        {
            return (await Connection.QueryAsync<T>(sql, parameters)).AsList();
        }
        finally
        {
            if (existingConnectionState != ConnectionState.Open)
                Connection.Close(); // Đóng kết nối nếu nó không mở trước đó
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

    public async Task<T?> QuerySingleAsync(string sql, object? parameters = null)
    {
        throw new NotImplementedException();
    }

    public async Task<int> ExecuteAsync(string sql, object? parameters = null)
    {
        var existingConnectionState = Connection.State;
        if (existingConnectionState != ConnectionState.Open)
            Connection.Open();

        try
        {
            return await Connection.ExecuteAsync(sql, parameters);
        }
        finally
        {
            if (existingConnectionState != ConnectionState.Open)
                Connection.Close();
        }
    }
}