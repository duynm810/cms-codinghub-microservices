using System.Net.Http.Headers;
using Media.Api.Services.Interfaces;
using Shared.Constants;
using Shared.Responses;
using Shared.Settings;
using Shared.Utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using ILogger = Serilog.ILogger;

namespace Media.Api.Services;

public class MediaService(IWebHostEnvironment hostEnvironment, MediaSettings mediaSettings, ILogger logger)
    : IMediaService
{
    public async Task<ApiResult<string>> UploadImage(IFormFile? file, string type)
    {
        var result = new ApiResult<string>();
        const string methodName = nameof(UploadImage);

        try
        {
            logger.Information("BEGIN {MethodName} - Starting image upload for Type: {Type}", methodName, type);

            // Check file is empty (Kiểm tra tập tin rỗng)
            if (file == null || file.Length == 0)
            {
                logger.Warning("Upload attempt with empty file.");
                result.Messages.Add(ErrorMessagesConsts.Media.FileIsEmpty);
                result.Failure(StatusCodes.Status400BadRequest, result.Messages);
                logger.Information("END {MethodName} - Failed to upload due to empty file.", methodName);
                return result;
            }

            // Extract and validate filename (Xác thực tên tập tin)
            var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"');
            if (string.IsNullOrEmpty(filename))
            {
                logger.Warning("Filename is empty after parsing.");
                result.Messages.Add(ErrorMessagesConsts.Media.FileNameCannotBeEmpty);
                result.Failure(StatusCodes.Status400BadRequest, result.Messages);
                logger.Information("END {MethodName} - Failed to upload due to empty filename.", methodName);
                return result;
            }

            // Get format file (Lấy định dạng của tập tin)
            var fileExtension = Path.GetExtension(filename).ToLower();
            if (!IsAllowedFileType(fileExtension))
            {
                logger.Warning("File extension {FileExtension} is not allowed.", fileExtension);
                result.Messages.Add(ErrorMessagesConsts.Media.InvalidFileTypeOrName);
                result.Failure(StatusCodes.Status400BadRequest, result.Messages);
                logger.Information("END {MethodName} - Failed to upload due to invalid file extension.", methodName);
                return result;
            }

            var now = DateTime.Now;
            var baseImageFolder = mediaSettings.ImageFolder ?? "DefaultImagePath";
            var imageFolder = Path.Combine(baseImageFolder, "images", type, now.ToString("MMyyyy"));

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
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = Path.Combine(folderPath, filename);

            try
            {
                // Process the image (Xử lý hình ảnh)
                await ProcessImage(file, filePath, fileExtension);
                logger.Information("Image processed and saved successfully.");

                var data = Path.Combine(imageFolder, filename).Replace("\\", "/");
                result.Success(data);

                logger.Information("END {MethodName} - Image uploaded and processed successfully for {Filename}.",
                    methodName, filename);
            }
            catch (Exception e)
            {
                result.Messages.AddRange(e.GetExceptionList());
                result.Failure(StatusCodes.Status400BadRequest, result.Messages);
            }
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(UploadImage), e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    #region HELPERS

    /// <summary>
    /// Check file extension is allowed
    /// </summary>
    /// <param name="fileExtension"></param>
    /// <returns></returns>
    private bool IsAllowedFileType(string fileExtension)
    {
        var allowImageTypes = mediaSettings.AllowImageFileTypes?.Split(",");
        return allowImageTypes != null &&
               allowImageTypes.Any(ext => fileExtension.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Process the image (Xử lý hình ảnh)
    /// </summary>
    /// <param name="file"></param>
    /// <param name="filePath"></param>
    /// <param name="fileExtension"></param>
    private async Task ProcessImage(IFormFile file, string filePath, string fileExtension)
    {
        using var image = await Image.LoadAsync(file.OpenReadStream());
        if (image.Width > 1920)
        {
            var newHeight = (int)(image.Height * (1920.0 / image.Width));
            image.Mutate(x => x.Resize(1920, newHeight));
        }

        // Determine the encoder based on file extension (Xác định bộ mã hóa dựa trên phần mở rộng tệp)
        IImageEncoder encoder = GetEncoder(fileExtension);
        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await image.SaveAsync(fileStream, encoder);
    }

    /// <summary>
    /// Select appropriate encoder for image format (Chọn bộ mã hóa phù hợp cho định dạng hình ảnh)
    /// </summary>
    /// <param name="fileExtension"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private IImageEncoder GetEncoder(string fileExtension)
    {
        return fileExtension switch
        {
            ".jpeg" or ".jpg" => new JpegEncoder { Quality = 75 },
            ".png" => new PngEncoder { CompressionLevel = PngCompressionLevel.BestCompression },
            ".gif" => new GifEncoder(),
            _ => throw new ArgumentException("Unsupported image format")
        };
    }

    #endregion
}