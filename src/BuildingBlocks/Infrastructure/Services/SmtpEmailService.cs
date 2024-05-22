using Contracts.Services.Interfaces;
using Infrastructure.Settings;
using MailKit.Net.Smtp;
using MimeKit;
using Serilog;
using Shared.Services.Email;

namespace Infrastructure.Services;

public class SmtpEmailService(ILogger logger, SmtpEmailSettings smtpEmailSettings) : ISmtpEmailService
{
    private readonly SmtpClient _smtpClient = new();

    public async Task SendEmailAsync(MailRequest request, CancellationToken cancellationToken = new())
    {
        var emailMessage = GetMimeMessage(request);

        try
        {
            await _smtpClient.ConnectAsync(smtpEmailSettings.SmtpServer, smtpEmailSettings.Port, smtpEmailSettings.UseSsl, cancellationToken);
            await _smtpClient.AuthenticateAsync(smtpEmailSettings.Username, smtpEmailSettings.Password, cancellationToken);
            await _smtpClient.SendAsync(emailMessage, cancellationToken);
            await _smtpClient.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.Error(ex.Message, ex);
        }
        finally
        {
            await _smtpClient.DisconnectAsync(true, cancellationToken);
            _smtpClient.Dispose();
        }
    }

    public void SendEmail(MailRequest request)
    {
        var emailMessage = GetMimeMessage(request);
        
        try
        {
            _smtpClient.Connect(smtpEmailSettings.SmtpServer, smtpEmailSettings.Port, smtpEmailSettings.UseSsl);
            _smtpClient.Authenticate(smtpEmailSettings.Username, smtpEmailSettings.Password);
            _smtpClient.Send(emailMessage);
            _smtpClient.Disconnect(true);
        }
        catch (Exception ex)
        {
            logger.Error(ex.Message, ex);
        }
        finally
        {
            _smtpClient.Disconnect(true);
            _smtpClient.Dispose();
        }
    }

    private MimeMessage GetMimeMessage(MailRequest request)
    {
        var emailMessage = new MimeMessage
        {
            Sender = new MailboxAddress(smtpEmailSettings.DisplayName, smtpEmailSettings.From),
            Subject = request.Subject,
            Body = new BodyBuilder
            {
                HtmlBody = request.Body
            }.ToMessageBody()
        };

        if (request.ToAddresses.Any())
        {
            foreach (var toAddress in request.ToAddresses)
            {
                emailMessage.To.Add(MailboxAddress.Parse(toAddress));
            }
        }
        else
        {
            var toAddress = request.ToAddress;
            emailMessage.To.Add(MailboxAddress.Parse(toAddress));
        }

        return emailMessage;
    }
}