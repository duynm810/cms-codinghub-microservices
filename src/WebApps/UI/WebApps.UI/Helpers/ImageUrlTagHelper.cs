using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using Shared.Settings;

namespace WebApps.UI.Helpers;

public class ImageUrlTagHelper(IOptions<ApiSettings> apiSettings) : TagHelper
{
    private readonly ApiSettings _apiSettings = apiSettings.Value;

    [HtmlAttributeName("src")] 
    public string? Src { get; set; }

    [HtmlAttributeName("id")] 
    public string? Id { get; set; }

    [HtmlAttributeName("class")]
    public string? AdditionalClass { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var fullUrl = $"{_apiSettings.ServerUrl}/media/get-image/{Src}";

        output.TagName = "img"; // Replace <image-url> with <img> tag
        output.Attributes.SetAttribute("src", fullUrl);

        var classValue = $"{AdditionalClass}";
      
        output.Attributes.SetAttribute("class", classValue);

        if (!string.IsNullOrEmpty(Id))
        {
            output.Attributes.SetAttribute("id", Id);
        }
    }
}