using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WebApps.UI.Helpers;

public class ImageUrlTagHelper(IConfiguration configuration) : TagHelper
{
    public string? Src { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var ocelotGatewayUrl = configuration["ApiSettings:ServerUrl"];
        var fullUrl = $"{ocelotGatewayUrl}/{Src}";

        output.TagName = "img"; // Replace <image-url> with <img> tag
        output.Attributes.SetAttribute("src", fullUrl);
        output.Attributes.SetAttribute("class", "border-radius-5");
    }
}