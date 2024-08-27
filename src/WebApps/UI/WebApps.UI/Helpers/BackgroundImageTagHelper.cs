using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using Shared.Settings;

namespace WebApps.UI.Helpers;

[HtmlTargetElement("div", Attributes = "thumbnail")]
public class BackgroundImageTagHelper(IOptions<ApiSettings> apiSettings) : TagHelper
{
    private readonly ApiSettings _apiSettings = apiSettings.Value;
    
    public string? ThumbnailFileId { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var fullUrl = $"{_apiSettings.ServerUrl}/{ThumbnailFileId}";

        if (output.Attributes.TryGetAttribute("style", out var styleAttribute))
        {
            var styleValue = styleAttribute.Value.ToString();
            var newStyleValue = $"background-image: url({fullUrl});{styleValue}";
            output.Attributes.SetAttribute("style", newStyleValue);
        }
        else
        {
            output.Attributes.SetAttribute("style", $"background-image: url({fullUrl});");
        }
    }
}