namespace WebApps.UI.CustomServices.Interfaces;

public interface IErrorService
{
    string GetErrorMessage(int statusCode);
    
    string GetViewName(int statusCode);
}