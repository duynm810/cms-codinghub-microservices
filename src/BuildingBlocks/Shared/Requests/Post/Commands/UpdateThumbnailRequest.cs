namespace Shared.Requests.Post.Commands;

public class UpdateThumbnailRequest
{
    public string? Thumbnail { get; set; }
    
    public string? ThumbnailFileId { get; set; }
}