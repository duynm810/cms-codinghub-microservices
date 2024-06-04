using System.ComponentModel.DataAnnotations;

namespace Shared.Services.Email;

public class MailRequest
{
    [EmailAddress] public required string ToAddress { get; set; }

    public IEnumerable<string> ToAddresses { get; set; } = new List<string>();

    [Required] public required string Subject { get; set; }

    [Required] public required string Body { get; set; }
}