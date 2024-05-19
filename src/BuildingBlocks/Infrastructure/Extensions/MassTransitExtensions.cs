using System.Reflection;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shared.Configurations;

namespace Infrastructure.Extensions;

public static class MassTransitExtensions
{
    public static void AddMassTransitWithRabbitMq(this IServiceCollection services)
    {
        var eventBusSettings = services.GetOptions<EventBusSettings>(nameof(EventBusSettings)) ??
                               throw new ArgumentNullException(
                                   $"{nameof(EventBusSettings)} is not configured properly");

        var mqConnection = new Uri(eventBusSettings.HostAddress);

        services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);

        services.AddMassTransit(config =>
        {
            config.AddConsumers(Assembly.GetEntryAssembly());
            config.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(mqConnection);
                cfg.ConfigureEndpoints(ctx, new KebabCaseEndpointNameFormatter(eventBusSettings.ServiceName, false));
                cfg.UseMessageRetry(retryConfigurator => { retryConfigurator.Interval(3, TimeSpan.FromSeconds(5)); });
            });
        });
    }
}