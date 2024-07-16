using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using Shared.Settings;

namespace WebApps.UI.Helpers;

[HtmlTargetElement("script", Attributes = "server-url")]
public class ServerUrlTagHelper(IOptions<ApiSettings> apiSettings) : TagHelper
{
    private readonly ApiSettings _apiSettings = apiSettings.Value;
    
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var scriptContent = $"const serverUrl = '{_apiSettings.ServerUrl}:{_apiSettings.Port}';";

        output.TagName = "script"; // Replace with <script> tag
        output.Content.SetHtmlContent(scriptContent);
    }
}