namespace Shared.Requests.Post.Commands;

public class TogglePinStatusRequest
{
    public bool IsPinned { get; set; }

    public int CurrentPage { get; set; }
}