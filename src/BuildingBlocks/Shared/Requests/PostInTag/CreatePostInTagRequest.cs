namespace Shared.Requests.PostInTag;

public class CreatePostInTagRequest
{
    public Guid TagId { get; set; }

    public Guid PostId { get; set; }

    public int SortOrder { get; set; }
}