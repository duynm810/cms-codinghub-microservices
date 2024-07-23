using Shared.Dtos.Aggregator;
using WebApps.UI.ApiClients.Interfaces;

namespace WebApps.UI.ApiClients;

public class AggregatorApiClient(IBaseApiClient baseApiClient) : IAggregatorApiClient
{
    public async Task<DashboardDto> GetDashboard()
    {
        return await baseApiClient.GetAsyncWithoutApiResult<DashboardDto>($"/dashboard");
    }

    public async Task<FooterDto> GetFooter()
    {
        return await baseApiClient.GetAsyncWithoutApiResult<FooterDto>($"/footer");
    }
    
    public async Task<SidebarDto> GetSidebar()
    {
        return await baseApiClient.GetAsyncWithoutApiResult<SidebarDto>($"/side-bar");
    }
}