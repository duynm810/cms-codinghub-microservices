using Contracts.Services.Interfaces;
using Shared.Settings;

namespace Infrastructure.Services;

public class EmailTemplateService(EmailTemplateSettings emailTemplateSettings) : IEmailTemplateService
{
    public string ReadEmailTemplate(string templateName)
    {
        var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, emailTemplateSettings.TemplateDirectory, $"{templateName}.html");
        return File.ReadAllText(templatePath);
    }
}