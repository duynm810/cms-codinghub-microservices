using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebApps.UI.ApiServices.Interfaces;
using WebApps.UI.Models.Commons;
using WebApps.UI.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.Components;

public class FooterViewComponent(ICategoryApiClient categoryApiClient, ITagApiClient tagApiClient, IErrorService errorService, ILogger logger)
    : BaseViewComponent(errorService, logger)
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        try
        {
            var categories = await categoryApiClient.GetCategories();
            var tags = await tagApiClient.GetTags(5);
            if (categories is { IsSuccess: true, Data: not null } && tags is { IsSuccess: true, Data: not null })
            {
                var viewModel = new FooterViewModel
                {
                    Categories = categories.Data,
                    Tags = tags.Data
                };

                return View(viewModel);
            }

            return HandleError((HttpStatusCode)categories.StatusCode, nameof(InvokeAsync));
        }
        catch (Exception e)
        {
            return HandleException(e, nameof(InvokeAsync));
        }
    }
}