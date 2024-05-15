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
        // Create a new MailRequest object with the provided email, subject, and email content
        var emailRequest = new MailRequest
        {
            ToAddress = to,
            Body = emailContent,
            Subject = subject
        };

        try
        {
            // Schedule the email to be sent at the specified time
            var jobId = scheduledJobService.Schedule(() => smtpEmailService.SendEmail(emailRequest), enqueueAt);
            
            logger.Information("Sent email to {Email} with subject: {Subject} - Job Id: {JobId}", to, subject,
                jobId);

            return jobId;
        }
        catch (Exception e)
        {
            logger.Error("failed due to an error with the email service: {ExMessage}", e.Message);
        }

        // Return null if the email is scheduled successfully
        return null;
    }
}