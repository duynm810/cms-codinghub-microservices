namespace Shared.Requests.Post.Commands;

public class ToggleFeaturedStatusRequest
{
    public bool IsFeatured { get; set; }
    
    public int CurrentPage { get; set; }
}