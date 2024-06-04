namespace WebApps.UI.Services.Interfaces;

public interface IErrorService
{
    string GetErrorMessage(int statusCode);

    string GetViewName(int statusCode);
}