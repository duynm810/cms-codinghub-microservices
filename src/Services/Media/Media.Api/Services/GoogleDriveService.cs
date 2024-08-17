using System.Net;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Media.Api.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace Media.Api.Services;

public class GoogleDriveService(ILogger logger) : IGoogleDriveService
{
    public DriveService GetService()
    {
        var credentialPath = Path.Combine(Directory.GetCurrentDirectory(), "Keys", "clean-carrier-432816-h3-b45ee5842fcc.json");

        GoogleCredential credential;
        using (var stream = new FileStream(credentialPath, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream)
                .CreateScoped(DriveService.Scope.DriveFile);
        }

        var service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "Coding Hub Microservices Web Application",
        });

        return service;
    }

    public async Task<string> UploadFile(Stream fileStream, string fileName, string mimeType, string folderId)
    {
        var service = GetService();
        const string methodName = nameof(UploadFile);

        try
        {
            logger.Information("BEGIN {MethodName} - Uploading file {FileName} to Google Drive", methodName, fileName);

            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = fileName,
                Parents = new List<string> { folderId }
            };

            var request = service.Files.Create(fileMetadata, fileStream, mimeType);
            request.Fields = "id";
            await request.UploadAsync();

            var file = request.ResponseBody;

            if (file == null)
            {
                logger.Error("END {MethodName} - Failed to upload file {FileName} to Google Drive", methodName, fileName);
                throw new Exception("Failed to upload file to Google Drive.");
            }

            logger.Information("{MethodName} - File uploaded successfully with ID: {FileId}", methodName, file.Id);

            // Share public files
            var permission = new Google.Apis.Drive.v3.Data.Permission
            {
                Role = "reader",
                Type = "anyone"
            };

            await service.Permissions.Create(permission, file.Id).ExecuteAsync();
            logger.Information("{MethodName} - File {FileName} is now publicly accessible.", methodName, fileName);

            return $"https://drive.google.com/uc?id={file.Id}";
        }
        catch (Exception e)
        {
            logger.Error("{MethodName} - Error while uploading file {FileName}: {ErrorMessage}", methodName, fileName, e.Message);
            throw;
        }
        finally
        {
            logger.Information("END {MethodName} - Upload process completed for file {FileName}", methodName, fileName);
        }
    }

    public async Task<bool> DeleteFile(string fileId)
    {
        var service = GetService();
        const string methodName = nameof(DeleteFile);

        try
        {
            logger.Information("BEGIN {MethodName} - Deleting file with ID: {FileId} from Google Drive", methodName, fileId);

            await service.Files.Delete(fileId).ExecuteAsync();

            logger.Information("END {MethodName} - File with ID: {FileId} deleted successfully", methodName, fileId);
            return true;
        }
        catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == HttpStatusCode.NotFound)
        {
            logger.Warning("{MethodName} - File with ID: {FileId} not found on Google Drive", methodName, fileId);
            return false;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName} - Error while deleting file with ID: {FileId}: {ErrorMessage}", methodName, fileId, e.Message);
            throw;
        }
    }
}