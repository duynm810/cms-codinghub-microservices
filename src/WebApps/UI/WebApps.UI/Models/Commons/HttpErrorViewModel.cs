using System.Net;

namespace WebApps.UI.Models.Commons;

public class HttpErrorViewModel : BaseViewModel
{
    public int? Code { get; set; }

    public HttpStatusCode? HttpStatusCode { get; set; }

    public List<string> Alternates { get; set; } = new List<string>();

    public HttpErrorViewModel(int? code)
    {
        Code = code;
        ShowSiteBottom = false; // Set to false for error pages

        if (code.HasValue && Enum.TryParse<HttpStatusCode>(Code.ToString(), out var httpStatusCode))
        {
            HttpStatusCode = httpStatusCode;
            Alternates.Add($"HttpError__{Code}");
            Alternates.Add($"HttpError__{httpStatusCode}");
        }
    }
}