namespace Shared.Settings;

public class IdentityServerSettings
{
    public string AuthorityUrl { get; set; } = default!;

    public string ClientId { get; set; } = default!;

    public string ClientSecret { get; set; } = default!;
}