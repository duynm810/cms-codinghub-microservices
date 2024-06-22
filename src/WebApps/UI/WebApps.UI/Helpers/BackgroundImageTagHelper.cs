using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WebApps.UI.Helpers;

[HtmlTargetElement("div", Attributes = "thumbnail")]
public class BackgroundImageTagHelper(IConfiguration configuration) : TagHelper
{
    public string? Thumbnail { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var ocelotGatewayUrl = configuration["ApiSettings:ServerUrl"];
        var fullUrl = $"{ocelotGatewayUrl}/{Thumbnail}";

        if (output.Attributes.TryGetAttribute("style", out var styleAttribute))
        {
            var styleValue = styleAttribute.Value.ToString();
            var newStyleValue = $"background-image: url({fullUrl}); {styleValue}";
            output.Attributes.SetAttribute("style", newStyleValue);
        }
        else
        {
            output.Attributes.SetAttribute("style", $"background-image: url({fullUrl});");
        }
    }
}