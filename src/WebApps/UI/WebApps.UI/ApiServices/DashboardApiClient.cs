using Contracts.Commons.Interfaces;
using Shared.Constants;
using Shared.Dtos.Dashboard;
using WebApps.UI.ApiServices.Interfaces;

namespace WebApps.UI.ApiServices;

public class DashboardApiClient(IBaseApiClient baseApiClient, ISerializeService serializeService) : IDashboardApiClient
{
    public async Task<DashboardDto> GetDashboard()
    {
        var client = await baseApiClient.CreateClientAsync(false);
        var response = await client.GetAsync($"/dashboard");

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = serializeService.Deserialize<DashboardDto>(responseContent);

        if (result == null)
        {
            throw new InvalidOperationException(ErrorMessagesConsts.Data.DeserializeFailed);
        }

        return result;
    }
}