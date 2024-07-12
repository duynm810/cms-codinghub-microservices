using Shared.Dtos.Dashboard;

namespace WebApps.UI.ApiClients.Interfaces;

public interface IDashboardApiClient
{
    Task<DashboardDto> GetDashboard();
}