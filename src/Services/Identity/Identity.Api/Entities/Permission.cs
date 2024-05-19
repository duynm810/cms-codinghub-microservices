using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Contracts.Domains;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Entities;

public class Permission(string function, string command, string roleId) : EntityBase<long>
{
    public Permission(long id, string function, string command, string roleId) : this(function, command, roleId)
    {
        Id = id;
    }

    [Key]
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string Function { get; set; } = function.ToUpper();

    [Key]
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string Command { get; set; } = command.ToUpper();

    [Required]
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string RoleId { get; set; } = roleId;

    [ForeignKey("RoleId")] 
    public virtual IdentityRole Role { get; set; }
}