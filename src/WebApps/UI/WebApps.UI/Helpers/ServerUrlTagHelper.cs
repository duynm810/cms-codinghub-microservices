using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WebApps.UI.Helpers;

[HtmlTargetElement("script", Attributes = "server-url")]
public class ServerUrlTagHelper(IConfiguration configuration) : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var ocelotGatewayUrl = configuration["ApiSettings:ServerUrl"];
        var scriptContent = $"const serverUrl = '{ocelotGatewayUrl}';";

        output.TagName = "script"; // Replace with <script> tag
        output.Content.SetHtmlContent(scriptContent);
    }
}