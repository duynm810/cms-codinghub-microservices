namespace Shared.Dtos.Tag;

public class TagDto
{
    public Guid Id { get; set; }
    
    public required string Name { get; set; }
    
    public string? Description { get; set; }
}