namespace Shared.Requests.Post.Commands;

public class RejectPostWithReasonRequest
{
    public string? Reason { get; set; }
    
    public int CurrentPage { get; set; }
}