using System.Reflection;
using Contracts.Services.Interfaces;
using FluentValidation;
using Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Post.Application.Commons.Behaviours;
using Post.Application.Commons.Mappings;
using Post.Application.Commons.Mappings.Interfaces;

namespace Post.Application;

public static class ConfigureServices
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        // Register AutoMapper
        services.AddAutoMapperConfiguration();

        // Register Validators
        services.AddValidatorServices();

        // Register MediatR for event handling
        services.AddMediatRConfiguration();

        // Register pipeline behaviors
        services.AddPipelineBehaviors();
    
        // Register infrastructure services
        services.AddAppInfrastructureServices();
    }

    private static void AddAutoMapperConfiguration(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddTransient<IMappingHelper,MappingHelper>();
    }

    private static void AddValidatorServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private static void AddMediatRConfiguration(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
    }

    private static void AddPipelineBehaviors(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
    }

    private static void AddAppInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IEmailTemplateService, EmailTemplateService>();
    }
}