namespace Shared.Dtos.Tag;

public class RawTagDto
{
    public required string Id { get; set; } = string.Empty;
    
    public required string Name { get; set; }
    
    public required string Slug { get; set; }
    
    public bool IsExisting { get; set; }
}