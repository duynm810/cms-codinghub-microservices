using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebApps.UI.ApiClients.Interfaces;
using WebApps.UI.Models.Commons;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Components;

public class FooterViewComponent(IAggregatorApiClient aggregatorApiClient, ILogger logger) : BaseViewComponent(logger)
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        const string methodName = nameof(InvokeAsync);

        try
        {
            var items = new FooterViewModel();

            var response = await aggregatorApiClient.GetFooter();
            
            if (response.Categories.Data is { Count: > 0 } categories)
            {
                items.Categories = categories;
            }

            if (response.Tags.Data is { Count: > 0 } tags)
            {
                items.Tags = tags;
            }

            return View(items);
        }
        catch (Exception e)
        {
            return HandleException(methodName, e);
        }
    }
}