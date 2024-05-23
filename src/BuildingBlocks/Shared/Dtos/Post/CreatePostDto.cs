namespace Shared.Dtos.Post;

public class CreatePostDto : CreateOrUpdatePostDto
{
    public Guid AuthorUserId { get; set; }
}