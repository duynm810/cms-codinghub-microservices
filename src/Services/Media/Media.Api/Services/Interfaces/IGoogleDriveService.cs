using Google.Apis.Drive.v3;

namespace Media.Api.Services.Interfaces;

public interface IGoogleDriveService
{
    Task<string> UploadFile(Stream stream, string fileName, string mimeType, string folderId);
    
    Task<bool> DeleteFile(string fileId);

    Task<Stream?> GetFileStream(string fileId);
}