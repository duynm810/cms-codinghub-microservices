using Contracts.Configurations.Interfaces;

namespace Infrastructure.Configurations;

public class SmtpEmailSettings : IEmailSettings
{
    public required string DisplayName { get; set; }

    public bool EnableVerification { get; set; }

    public required string From { get; set; }

    public required string SmtpServer { get; set; }

    public bool UseSsl { get; set; }

    public int Port { get; set; }

    public required string Username { get; set; }

    public required string Password { get; set; }
}