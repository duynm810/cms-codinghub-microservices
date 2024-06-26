using System.Net;

namespace WebApps.UI.Models.Commons;

public class HttpErrorViewModel
{
    public int? Code { get; set; }

    public HttpStatusCode? HttpStatusCode { get; set; }

    public List<string> Alternates { get; set; } = new();

    public HttpErrorViewModel(int? code)
    {
        Code = code;
        
        if (!code.HasValue || !Enum.TryParse<HttpStatusCode>(Code.ToString(), out var httpStatusCode))
        {
            return;
        }
        
        HttpStatusCode = httpStatusCode;
        Alternates.Add($"HttpError__{Code}");
        Alternates.Add($"HttpError__{httpStatusCode}");
    }
}