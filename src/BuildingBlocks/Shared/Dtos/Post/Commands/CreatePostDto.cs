namespace Shared.Dtos.Post.Commands;

public class CreatePostDto : CreateOrUpdatePostDto
{
    public Guid AuthorUserId { get; set; }
}