using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Media.Api.Services.Interfaces;

namespace Media.Api.Services;

public class GoogleDriveService : IGoogleDriveService
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
}