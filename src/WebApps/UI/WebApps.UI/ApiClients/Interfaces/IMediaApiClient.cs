using Shared.Responses;

namespace WebApps.UI.ApiClients.Interfaces;

public interface IMediaApiClient
{
    Task<ApiResult<string>> UploadImage(IFormFile? file, string type);

    Task<ApiResult<bool>> DeleteImage(string imagePath);
}