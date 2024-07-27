using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using Shared.Settings;

namespace WebApps.UI.Helpers;

public class ImageUrlTagHelper(IOptions<ApiSettings> apiSettings) : TagHelper
{
    private readonly ApiSettings _apiSettings = apiSettings.Value;
    
    public string? Src { get; set; }
    public string? Id { get; set; }
    
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var fullUrl = !string.IsNullOrEmpty(_apiSettings.Port)
            ? $"{_apiSettings.ServerUrl}:{_apiSettings.Port}/{Src}" 
            : $"{_apiSettings.ServerUrl}/{Src}";

        output.TagName = "img"; // Replace <image-url> with <img> tag
        output.Attributes.SetAttribute("src", fullUrl);
        output.Attributes.SetAttribute("class", "border-radius-5");

        if (!string.IsNullOrEmpty(Id))
        {
            output.Attributes.SetAttribute("id", Id);
        }
    }
}