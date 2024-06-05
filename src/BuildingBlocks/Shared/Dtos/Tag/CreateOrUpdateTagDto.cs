namespace Shared.Dtos.Tag;

public class CreateOrUpdateTagDto
{
    public required string Name { get; set; }
    
    public string? Description { get; set; }
}