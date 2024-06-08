namespace Shared.Dtos.PostInTag;

public class CreatePostInTagDto
{
    public Guid TagId { get; set; }

    public Guid PostId { get; set; }

    public int SortOrder { get; set; }
}