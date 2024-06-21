using System.Net.Http.Headers;
using Contracts.Commons.Interfaces;
using Shared.Constants;
using Shared.Responses;
using Shared.Utilities;
using WebApps.UI.ApiServices.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.ApiServices;

public class MediaApiClient(IBaseApiClient baseApiClient, ISerializeService serializeService, ILogger logger)
    : IMediaApiClient
{
    public async Task<ApiResult<string>> UploadImage(IFormFile? file, string type)
    {
        var result = new ApiResult<string>();
        const string methodName = nameof(UploadImage);

        try
        {
            logger.Information("BEGIN {MethodName} - Starting image upload for Type: {Type}", methodName, type);

            // Check if file is null (Kiểm tra tập tin null)
            if (file == null || file.Length == 0)
            {
                logger.Warning("Upload attempt with empty file.");
                result.Messages.Add(ErrorMessagesConsts.Media.FileIsEmpty);
                result.Failure(StatusCodes.Status400BadRequest, result.Messages);
                logger.Information("END {MethodName} - Failed to upload due to empty file.", methodName);
                return result;
            }

            var client = await baseApiClient.CreateClientAsync(true);

            using var content = new MultipartFormDataContent();

            var fileContent = new StreamContent(file.OpenReadStream());
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            content.Add(fileContent, "File", file.FileName);
            content.Add(new StringContent(type), "Type");

            logger.Information("Sending POST request to upload-image API");

            var response = await client.PostAsync("media/upload-image", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                logger.Error(ErrorMessagesConsts.Media.ImageUploadFailed, response.StatusCode, errorContent);
                throw new HttpRequestException(string.Format(ErrorMessagesConsts.Network.RequestFailed, response.StatusCode, errorContent));
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var deserializedResult = serializeService.Deserialize<ApiResult<string>>(responseContent);

            if (deserializedResult != null)
            {
                return deserializedResult;
            }

            logger.Error(ErrorMessagesConsts.Data.DeserializeFailed);
            throw new InvalidOperationException(ErrorMessagesConsts.Data.DeserializeFailed);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
            return result;
        }
    }
}