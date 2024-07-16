using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using Shared.Settings;

namespace WebApps.UI.Helpers;

public class ImageUrlTagHelper(IOptions<ApiSettings> apiSettings) : TagHelper
{
    private readonly ApiSettings _apiSettings = apiSettings.Value;
    
    public string? Src { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var ocelotGatewayUrl = _apiSettings.ServerUrl;
        var fullUrl = $"{ocelotGatewayUrl}/{Src}";

        output.TagName = "img"; // Replace <image-url> with <img> tag
        output.Attributes.SetAttribute("src", fullUrl);
        output.Attributes.SetAttribute("class", "border-radius-5");
    }
}