using Google.Apis.Drive.v3;

namespace Media.Api.Services.Interfaces;

public interface IGoogleDriveService
{
    DriveService GetService();
}