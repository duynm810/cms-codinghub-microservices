using Contracts.Scheduled;
using Contracts.Services.Interfaces;
using Hangfire.Api.Services.Interfaces;
using Shared.Services.Email;
using ILogger = Serilog.ILogger;

namespace Hangfire.Api.Services;

public class BackgroundJobService(
    IScheduledJobService scheduledJobService,
    ISmtpEmailService smtpEmailService,
    ILogger logger) : IBackgroundJobService
{
    public string? SendEmail(string to, string subject, string emailContent, DateTimeOffset enqueueAt)
    {
        var emailRequest = new MailRequest
        {
            ToAddress = to,
            Body = emailContent,
            Subject = subject
        };

        try
        {
            var jobId = scheduledJobService.Schedule(() => smtpEmailService.SendEmail(emailRequest), enqueueAt);
            logger.Information("Scheduled email to {Email} with subject: {Subject} - Job Id: {JobId}", to, subject, jobId);
            return jobId;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Failed to schedule email to {Email} with subject: {Subject}", to, subject);
            return null;
        }
    }
}