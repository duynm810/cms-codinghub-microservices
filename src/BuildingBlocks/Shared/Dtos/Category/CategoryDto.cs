namespace Shared.Dtos.Category;

public class CategoryDto
{
    public required string Name { get; set; }

    public required string Slug { get; set; }

    public string? SeoDescription { get; set; }

    public Guid ParentId { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; }
}