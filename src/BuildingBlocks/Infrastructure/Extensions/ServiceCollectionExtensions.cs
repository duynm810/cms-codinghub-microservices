using Contracts.Domains.Repositories;
using Infrastructure.Domains;
using Infrastructure.Domains.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddCoreInfrastructure(this IServiceCollection services)
    {
        services
            .AddScoped(typeof(IRepositoryQueryBase<,,>), typeof(RepositoryQueryBase<,,>))
            .AddScoped(typeof(IRepositoryCommandBase<,,>), typeof(RepositoryCommandBase<,,>))
            .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
    }
}