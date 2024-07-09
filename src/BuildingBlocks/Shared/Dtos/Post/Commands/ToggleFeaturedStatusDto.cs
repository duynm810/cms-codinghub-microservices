namespace Shared.Dtos.Post.Commands;

public class ToggleFeaturedStatusDto
{
    public bool IsFeatured { get; set; }
    
    public int CurrentPage { get; set; }
}