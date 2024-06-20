using Shared.Responses;

namespace WebApps.UI.ApiServices.Interfaces;

public interface IMediaApiClient
{
    Task<ApiResult<string>> UploadImage(IFormFile? file, string type);
}