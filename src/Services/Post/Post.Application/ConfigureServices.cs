using System.Reflection;
using Contracts.Services.Interfaces;
using FluentValidation;
using Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Post.Application.Commons.Behaviours;

namespace Post.Application;

public static class ConfigureServices
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        // Configures and registers AutoMapper
        services.ConfigureAutoMapper();

        // Configures and registers Validators
        services.ConfigureValidators();

        // Configures and registers MediatR (using for event handler)
        services.ConfigureMediatR();

        // Configures and registers pipeline (middleware)
        services.ConfigurePipelineBehaviours();
        
        // Configures infrastructure services in infrastructure class library
        services.ConfigureInfrastructureServices();
    }

    private static void ConfigureAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
    }

    private static void ConfigureValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private static void ConfigureMediatR(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
    }

    private static void ConfigurePipelineBehaviours(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
    }

    private static void ConfigureInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IEmailTemplateService, EmailTemplateService>();
    }
}