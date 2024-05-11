using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Post.Domain.Entities;

namespace Post.Infrastructure.EntityConfigurations;

public class PostActivityLogConfiguration  : IEntityTypeConfiguration<PostActivityLog>
{
    public void Configure(EntityTypeBuilder<PostActivityLog> builder)
    {
        builder.HasOne<PostBase>().WithMany().HasForeignKey(x => x.PostId);
    }
}