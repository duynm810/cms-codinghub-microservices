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

            // Ensure wwwroot folder exists
            var wwwrootPath = hostEnvironment.WebRootPath;
            if (!Directory.Exists(wwwrootPath))
            {
                Directory.CreateDirectory(wwwrootPath);
            }

            // Ensure base image folder exists
            var baseFolderPath = Path.Combine(wwwrootPath, baseImageFolder);
            if (!Directory.Exists(baseFolderPath))
            {
                Directory.CreateDirectory(baseFolderPath);
            }

            var folderPath = Path.Combine(wwwrootPath, imageFolder);
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
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
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
    /// Helper method to calculate the corresponding height for a given width to maintain aspect ratio
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