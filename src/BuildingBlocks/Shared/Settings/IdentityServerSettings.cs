namespace Shared.Settings;

public class IdentityServerSettings
{
    public string? AuthorityUrl { get; set; }
    
    public string? IssuerUri { get; set; }

    public string? ClientId { get; set; }

    public string? ClientSecret { get; set; }
}