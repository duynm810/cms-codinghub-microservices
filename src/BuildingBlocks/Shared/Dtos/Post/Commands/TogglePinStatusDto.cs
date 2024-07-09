namespace Shared.Dtos.Post.Commands;

public class TogglePinStatusDto
{
    public bool IsPinned { get; set; }

    public int CurrentPage { get; set; }
}