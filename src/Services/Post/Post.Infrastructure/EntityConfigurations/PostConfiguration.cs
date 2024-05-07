using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Post.Domain.Entities;
using Shared.Enums;

namespace Post.Infrastructure.EntityConfigurations;

public class PostConfiguration : IEntityTypeConfiguration<PostBase>
{
    public void Configure(EntityTypeBuilder<PostBase> builder)
    {
        builder.Property(x => x.Status).HasDefaultValue(PostStatusEnum.Draft).IsRequired();
    }
}