using System.Net.Http.Headers;
using Contracts.Commons.Interfaces;
using Shared.Constants;
using Shared.Responses;
using WebApps.UI.ApiServices.Interfaces;
using ILogger = Serilog.ILogger;

namespace WebApps.UI.ApiServices;

public class MediaApiClient(IBaseApiClient baseApiClient, ISerializeService serializeService, ILogger logger) : IMediaApiClient
{
    public async Task<ApiResult<string>> UploadImage(IFormFile? file, string type)
    {
        if (file == null)
        {
            logger.Error(ErrorMessagesConsts.Media.FileNotUploaded);
            throw new ArgumentNullException(nameof(file));
        }
        
        logger.Information("Starting image upload...");
        
        var client = await baseApiClient.CreateClientAsync(true);

        using var content = new MultipartFormDataContent();
        
        var fileContent = new StreamContent(file.OpenReadStream());
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, "file", file.FileName);
        content.Add(new StringContent(type), "type");
        
        logger.Information("Sending POST request to upload-image API");
        
        var response = await client.PostAsync("media/upload-image", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            logger.Error(ErrorMessagesConsts.Media.ImageUploadFailed, response.StatusCode, errorContent);
            throw new HttpRequestException(string.Format(ErrorMessagesConsts.Network.RequestFailed, response.StatusCode, errorContent));
        }
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = serializeService.Deserialize<ApiResult<string>>(responseContent);

        if (result != null)
        {
            return result;
        }
        
        logger.Error(ErrorMessagesConsts.Data.DeserializeFailed);
        throw new InvalidOperationException(ErrorMessagesConsts.Data.DeserializeFailed);
    }
}