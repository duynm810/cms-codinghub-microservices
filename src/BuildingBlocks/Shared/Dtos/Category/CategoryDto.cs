namespace Shared.Dtos.Category;

public class CategoryDto
{
    public long Id { get; set; }

    public required string Name { get; set; }

    public required string Slug { get; set; }

    public string? SeoDescription { get; set; }

    public Guid? ParentId { get; set; }
    
    public string? Icon { get; set; }

    public string? Color { get; set; }

    public int SortOrder { get; set; }
    
    public bool IsStaticPage { get; set; }

    public bool IsActive { get; set; }
}