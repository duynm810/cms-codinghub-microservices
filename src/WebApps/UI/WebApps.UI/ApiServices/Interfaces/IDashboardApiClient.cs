using Shared.Dtos.Dashboard;

namespace WebApps.UI.ApiServices.Interfaces;

public interface IDashboardApiClient
{
    Task<DashboardDto> GetDashboard();
}