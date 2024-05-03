using Contracts.Domains.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Domains;

public class UnitOfWork<TContext>(TContext context) : IUnitOfWork<TContext>
    where TContext : DbContext
{
    public void Dispose() => context.Dispose();

    public Task<int> CommitAsync() => context.SaveChangesAsync();
}