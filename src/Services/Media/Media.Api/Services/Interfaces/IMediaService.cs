using Shared.Responses;

namespace Media.Api.Services.Interfaces;

public interface IMediaService
{
    Task<ApiResult<string>> UploadImage(IFormFile? file, string type);
}