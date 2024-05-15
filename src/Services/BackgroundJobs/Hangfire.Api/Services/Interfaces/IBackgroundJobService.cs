using Contracts.Scheduled;

namespace Hangfire.Api.Services.Interfaces;

public interface IBackgroundJobService
{
    string? SendEmail(string to, string subject, string emailContent, DateTimeOffset enqueueAt);
}