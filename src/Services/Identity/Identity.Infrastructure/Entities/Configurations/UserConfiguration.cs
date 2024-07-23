using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Entities.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.FirstName)
            .IsRequired(false)
            .HasColumnType("nvarchar(50)");

        builder.Property(u => u.LastName)
            .IsRequired(false)
            .HasColumnType("nvarchar(50)");

        builder.Property(u => u.Address)
            .IsRequired(false)
            .HasColumnType("nvarchar(250)");

        builder.Property(u => u.AvatarUrl)
            .IsRequired(false)
            .HasColumnType("varchar(500)");
    }
}