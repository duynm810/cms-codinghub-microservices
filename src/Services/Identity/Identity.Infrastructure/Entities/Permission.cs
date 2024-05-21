using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Contracts.Domains;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Entities;

public class Permission : EntityBase<long>
{
    // Constructor mặc định
    public Permission()
    {
        Function = string.Empty;
        Command = string.Empty;
        RoleId = string.Empty;
    }

    public Permission(string function, string command, string roleId)
    {
        Function = function.ToUpper();
        Command = command.ToUpper();
        RoleId = roleId;
    }

    public Permission(long id, string function, string command, string roleId) : this(function, command, roleId)
    {
        Id = id;
    }

    [Key]
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string Function { get; set; }

    [Key]
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string Command { get; set; }

    [Required]
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string RoleId { get; set; }

    [ForeignKey("RoleId")] public virtual IdentityRole? Role { get; set; }
}