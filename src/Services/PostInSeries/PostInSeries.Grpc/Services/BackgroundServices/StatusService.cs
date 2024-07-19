using Grpc.Health.V1;
using Grpc.HealthCheck;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PostInSeries.Grpc.Services.BackgroundServices;

public class StatusService(HealthServiceImpl healthService, HealthCheckService healthCheckService)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Check the health of the service (Kiểm tra sức khỏe dịch vụ)
            var health = await healthCheckService.CheckHealthAsync(stoppingToken);

            foreach (var h in health.Entries)
                healthService.SetStatus(h.Key,
                    health.Status == HealthStatus.Healthy
                        ? HealthCheckResponse.Types.ServingStatus.Serving
                        : HealthCheckResponse.Types.ServingStatus.NotServing);

            // Wait 15 seconds before checking again (Chờ 15 giây trước khi kiểm tra lại)
            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
        }
    }
}