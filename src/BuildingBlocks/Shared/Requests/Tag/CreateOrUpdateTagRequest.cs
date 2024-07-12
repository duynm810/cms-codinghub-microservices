namespace Shared.Requests.Tag;

public class CreateOrUpdateTagRequest
{
    public required string Name { get; set; }
    
    public required string Slug { get; set; }
    
    public string? Description { get; set; }
}