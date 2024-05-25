using System.Net.Http.Headers;
using Media.Api.Services.Interfaces;
using Shared.Constants;
using Shared.Responses;
using Shared.Settings;
using Shared.Utilities;
using ILogger = Serilog.ILogger;

namespace Media.Api.Services;

public class MediaService(IWebHostEnvironment hostEnvironment, MediaSettings mediaSettings, ILogger logger) : IMediaService
{
    public async Task<ApiResult<string>> UploadImage(IFormFile? file, string type)
    {
        var result = new ApiResult<string>();

        try
        {
            var baseImageFolder = mediaSettings.ImageFolder ?? "DefaultImagePath";
            var allowImageTypes = mediaSettings.AllowImageFileTypes?.Split(",");

            if (file == null || file.Length == 0)
            {
                result.Messages.Add(ErrorMessagesConsts.Media.FileIsEmpty);
                result.Failure(StatusCodes.Status400BadRequest, result.Messages);
                return result;
            }
            
            var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"');
            if (string.IsNullOrEmpty(filename) ||
                (allowImageTypes != null && !allowImageTypes.Any(x => filename.EndsWith(x, StringComparison.OrdinalIgnoreCase))))
            {
                result.Messages.Add(ErrorMessagesConsts.Media.InvalidFileTypeOrName);
                result.Failure(StatusCodes.Status400BadRequest, result.Messages);
                return result;
            }

            var now = DateTime.Now;
            var imageFolder = Path.Combine(baseImageFolder, "images", type, now.ToString("dd/MM/yyyy"));

            if (string.IsNullOrWhiteSpace(hostEnvironment.WebRootPath))
            {
                hostEnvironment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            if (string.IsNullOrEmpty(filename))
            {
                result.Messages.Add(ErrorMessagesConsts.Media.FileNameCannotBeEmpty);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var folderPath = Path.Combine(hostEnvironment.WebRootPath, imageFolder);
            Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, filename);
            await using (var fs = new FileStream(filePath, FileMode.Create))
            await using (var bufferedStream = new BufferedStream(fs))
            {
                await file.CopyToAsync(bufferedStream);
            }

            var data = Path.Combine(imageFolder, filename).Replace("\\", "/");

            result.Success(data);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(UploadImage), e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }
}