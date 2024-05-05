using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Contracts.Domains;
using Microsoft.EntityFrameworkCore;

namespace Category.API.Entities;

[Table("Categories")]
[Index(nameof(Slug), IsUnique = true)]
public class CategoryBase : EntityAuditBase<Guid>
{
    [MaxLength(250)] 
    public required string Name { get; set; }

    [Column(TypeName = "varchar(250)")] 
    public required string Slug { get; set; }

    [MaxLength(150)] 
    public string? SeoDescription { get; set; }

    public Guid? ParentId { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; }
}