using Shared.Services.Email;

namespace Contracts.Services.Interfaces;

public interface ISmtpEmailService : IEmailService<MailRequest>
{
    
}