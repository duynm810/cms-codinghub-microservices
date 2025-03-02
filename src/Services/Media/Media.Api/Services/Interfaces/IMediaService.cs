using Media.Api.Dtos;
using Shared.Responses;

namespace Media.Api.Services.Interfaces;

public interface IMediaService
{
    Task<ApiResult<string>> UploadImage(SingleFileDto request);

    ApiResult<bool> DeleteImage(string imagePath);
}