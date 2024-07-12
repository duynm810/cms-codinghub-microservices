namespace Shared.Requests.PostInTag;

public class DeletePostInTagRequest
{
    public Guid TagId { get; set; }

    public Guid PostId { get; set; }
}