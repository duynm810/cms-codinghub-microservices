using System.Net.Http.Headers;
using Google.Apis.Drive.v3;
using Media.Api.Dtos;
using Media.Api.Services.Interfaces;
using Microsoft.Extensions.Options;
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

public class MediaService(IWebHostEnvironment hostEnvironment, IGoogleDriveService googleDriveService, GoogleDriveSettings googleDriveSettings, MediaSettings mediaSettings, ILogger logger)
    : IMediaService
{
    public async Task<ApiResult<string>> UploadImage(SingleFileDto request)
    {
        var result = new ApiResult<string>();
        const string methodName = nameof(UploadImage);

        try
        {
            logger.Information("BEGIN {MethodName} - Starting image upload for Type: {Type}", methodName, request.Type);

            // Check file is empty
            if (request.File == null || request.File.Length == 0)
            {
                logger.Warning("Upload attempt with empty file.");
                result.Messages.Add(ErrorMessagesConsts.Media.FileIsEmpty);
                result.Failure(StatusCodes.Status400BadRequest, result.Messages);
                return result;
            }

            // Extract and validate filename
            var filename = ContentDispositionHeaderValue.Parse(request.File.ContentDisposition).FileName?.Trim('"');
            if (string.IsNullOrEmpty(filename))
            {
                logger.Warning("Filename is empty after parsing.");
                result.Messages.Add(ErrorMessagesConsts.Media.FileNameCannotBeEmpty);
                result.Failure(StatusCodes.Status400BadRequest, result.Messages);
                return result;
            }

            // Get format file
            var fileExtension = Path.GetExtension(filename).ToLower();
            if (!IsAllowedFileType(fileExtension))
            {
                logger.Warning("File extension {FileExtension} is not allowed.", fileExtension);
                result.Messages.Add(ErrorMessagesConsts.Media.InvalidFileTypeOrName);
                result.Failure(StatusCodes.Status400BadRequest, result.Messages);
                return result;
            }

            var now = DateTime.Now;
            var baseImageFolder = mediaSettings.ImageFolder;
            var imageFolder = Path.Combine(baseImageFolder, "images", request.Type ?? "unkown", now.ToString("ddMMyyyy"));

            // Ensure wwwroot folder exists
            var wwwrootPath = hostEnvironment.WebRootPath;
            if (string.IsNullOrWhiteSpace(wwwrootPath))
            {
                wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                Utils.EnsureDirectoryExists(wwwrootPath);
                logger.Information("wwwroot path set to: {wwwrootPath}", wwwrootPath);
            }
            else
            {
                logger.Information("Using provided wwwroot path: {wwwrootPath}", wwwrootPath);
            }
            
            // Ensure base image folder exists
            var baseFolderPath = Path.Combine(wwwrootPath, baseImageFolder);
            Utils.EnsureDirectoryExists(baseFolderPath);
            logger.Information("Base image folder path: {baseFolderPath}", baseFolderPath);

            var folderPath = Path.Combine(wwwrootPath, imageFolder);
            Utils.EnsureDirectoryExists(folderPath);
            logger.Information("Specific image folder path: {folderPath}", folderPath);

            var filePath = Path.Combine(folderPath, filename);
            logger.Information("Full file path: {filePath}", filePath);

            try
            {
                // Process the image
                /*await ProcessImage(request.File, filePath, fileExtension);*/
                await using var fileStream = new FileStream(filePath, FileMode.Create);
                await request.File.CopyToAsync(fileStream);
                
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
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public ApiResult<bool> DeleteImage(string imagePath)
    {
        var result = new ApiResult<bool>();
        const string methodName = nameof(DeleteImage);
        
        try
        {
            logger.Information("BEGIN {MethodName} - Deleting image at path: {ImagePath}", methodName, imagePath);

            // Ensure wwwroot folder exists
            var wwwrootPath = hostEnvironment.WebRootPath;
            if (string.IsNullOrWhiteSpace(wwwrootPath))
            {
                wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                Utils.EnsureDirectoryExists(wwwrootPath);
                logger.Information("wwwroot path set to: {wwwrootPath}", wwwrootPath);
            }
            else
            {
                logger.Information("Using provided wwwroot path: {wwwrootPath}", wwwrootPath);
            }
            
            // Decode the URL-encoded path
            var decodedImagePath = Uri.UnescapeDataString(imagePath);
            logger.Information("Decoded imagePath: {DecodedImagePath}", decodedImagePath);

            var fullPath = Path.Combine(wwwrootPath, decodedImagePath);
            logger.Information("Computed fullPath: {FullPath}", fullPath);
            
            if (!File.Exists(fullPath))
            {
                logger.Warning("Image not found at path: {ImagePath}", fullPath);
                result.Messages.Add("Image not found.");
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }
            
            File.Delete(fullPath);
            logger.Information("Image successfully deleted at path: {ImagePath}", fullPath);

            result.Success(true);
            logger.Information("END {MethodName} - Image deleted successfully.", methodName);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<string>> UploadImageToGoogleDrive(SingleFileDto request)
    {
        var result = new ApiResult<string>();
        const string methodName = nameof(UploadImageToGoogleDrive);
        
        try
        {
            logger.Information("BEGIN {MethodName} - Starting image upload to Google Drive for Type: {Type}", methodName, request.Type);

            // Check file is empty
            if (request.File == null || request.File.Length == 0)
            {
                logger.Warning("Upload attempt with empty file.");
                result.Messages.Add(ErrorMessagesConsts.Media.FileIsEmpty);
                result.Failure(StatusCodes.Status400BadRequest, result.Messages);
                return result;   
            }
            
            // Extract and validate filename
            var filename = ContentDispositionHeaderValue.Parse(request.File.ContentDisposition).FileName?.Trim('"');
            if (string.IsNullOrEmpty(filename))
            {
                logger.Warning("Filename is empty after parsing.");
                result.Messages.Add(ErrorMessagesConsts.Media.FileNameCannotBeEmpty);
                result.Failure(StatusCodes.Status400BadRequest, result.Messages);
                return result;
            }
            
            // Get format file
            var fileExtension = Path.GetExtension(filename).ToLower();
            if (!IsAllowedFileType(fileExtension))
            {
                logger.Warning("File extension {FileExtension} is not allowed.", fileExtension);
                result.Messages.Add(ErrorMessagesConsts.Media.InvalidFileTypeOrName);
                result.Failure(StatusCodes.Status400BadRequest, result.Messages);
                return result;
            }
            
            var service = googleDriveService.GetService();
            
            var folderId = string.Empty;
            switch (request.Type)
            {
                case "posts":
                    folderId = googleDriveSettings.PostsFolderId;
                    break;
                case "avatar":
                    folderId = googleDriveSettings.AvatarsFolderId;
                    break;
            }
            
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = filename,
                Parents = new List<string> { folderId }
            };

            await using var stream = request.File.OpenReadStream();
            var requestDrive = service.Files.Create(fileMetadata, stream, "image/jpeg");
            requestDrive.Fields = "id";
            
            await requestDrive.UploadAsync();
            
            var file = requestDrive.ResponseBody;
            if (file != null)
            {
                // Share public file just upload
                var permission = new Google.Apis.Drive.v3.Data.Permission
                {
                    Role = "reader",
                    Type = "anyone"
                };
                await service.Permissions.Create(permission, file.Id).ExecuteAsync();

                // Create url for images just upload
                var fileUrl = $"https://drive.google.com/uc?id={file.Id}";
                result.Success(fileUrl);
                logger.Information("Image uploaded successfully to Google Drive. File ID: {FileId}", file.Id);
            }
            else
            {
                result.Messages.Add("Failed to upload image to Google Drive.");
                result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
            }
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public ApiResult<bool> DeleteImageFromGoogleDrive(string fileId)
    {
        throw new NotImplementedException();
    }

    #region HELPERS

    /// <summary>
    /// Check file extension is allowed
    /// </summary>
    /// <param name="fileExtension"></param>
    /// <returns></returns>
    private bool IsAllowedFileType(string fileExtension)
    {
        var allowImageTypes = mediaSettings.AllowImageFileTypes.Split(",");
        return allowImageTypes.Any(ext => fileExtension.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
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
        var encoder = GetEncoder(fileExtension); // Determine the appropriate encoder (Xác định bộ mã hóa thích hợp)
        const int
            maxFileSize =
                500000; // Maximum file size in bytes, for example 500 KB (Kích thước tối đa tính bằng byte, ví dụ 500 KB)
        const int maxWidth = 1920; // Maximum width to start (Chiều rộng hình ảnh tối đa để bắt đầu xử lý tối ưu)

        // Use binary search to find the optimal size (Sử dụng tìm kiếm nhị phân để tìm kích thước tối ưu)
        var min = 0;
        var max = maxWidth;
        var bestWidth = max;
        var bestHeight =
            CalculateHeight(image, bestWidth); // Calculate initial best height (Tính chiều cao tốt nhất ban đầu)

        using var tmpStream = new MemoryStream();
        while (min <= max)
        {
            var width = (min + max) / 2;
            var height = CalculateHeight(image, width);
            tmpStream.SetLength(0); // Reset the memory stream for reuse (Đặt lại luồng bộ nhớ để sử dụng lại)

            // Adjust the temporary size to this width and height (Điều chỉnh kích thước tạm thời với chiều rộng và chiều cao này)
            using var resizedImage = image.Clone(op => op.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(width, height)
            }));

            // Save to stream to check file size (Lưu vào luồng tạm thời để kiểm tra kích thước tệp)
            await resizedImage.SaveAsync(tmpStream, encoder);

            // Nếu kích thước tệp nhỏ hơn maxFileSize, chiều rộng và chiều cao này được xem là khả thi
            if (tmpStream.Length < maxFileSize)
            {
                bestWidth = width;
                bestHeight = height;
                min = width + 1; // The algorithm will try a larger size (thuật toán sẽ thử một kích thước lớn hơn)
            }
            else
            {
                max = width -
                      1; // If the file size is over the limit, try a smaller size (Nếu kích thước tệp lớn hơn, thử lại kích thước nhỏ hơn)
            }
        }

        // Resize to the best width and height found and save to the final destination (Thay đổi kích thước theo chiều rộng và chiều cao tốt nhất được tìm thấy và lưu vào đích cuối cùng)
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Max,
            Size = new Size(bestWidth, bestHeight)
        }));

        await using var finalStream = new FileStream(filePath, FileMode.Create);
        await image.SaveAsync(finalStream, encoder);
    }

    /// <summary>
    /// Helper method to calculate the corresponding height for a given width to maintain aspect ratio (Phương pháp trợ giúp để tính chiều cao tương ứng cho chiều rộng nhất định để duy trì tỷ lệ khung hình)
    /// </summary>
    /// <param name="image"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    private int CalculateHeight(Image image, int width)
    {
        return (int)(image.Height * (width / (double)image.Width));
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