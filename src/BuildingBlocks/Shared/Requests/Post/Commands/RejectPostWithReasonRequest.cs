namespace Shared.Requests.Post.Commands;

public class RejectPostWithReasonRequest
{
    public string? Note { get; set; }
    
    public int CurrentPage { get; set; }
}