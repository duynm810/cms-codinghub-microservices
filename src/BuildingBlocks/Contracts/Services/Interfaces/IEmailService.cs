namespace Contracts.Services.Interfaces;

public interface IEmailService<in T> where T : class
{
    Task SendEmailAsync(T request, CancellationToken cancellationToken = new());

    void SendEmail(T request);
}