using Shared.Dtos.Aggregator;

namespace WebApps.UI.ApiClients.Interfaces;

public interface IAggregatorApiClient
{
    Task<DashboardDto> GetDashboard();

    Task<FooterDto> GetFooter();

    Task<SidebarDto> GetSidebar();
}