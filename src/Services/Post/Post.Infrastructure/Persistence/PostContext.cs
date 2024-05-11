using Contracts.Domains.Interfaces;
using Microsoft.EntityFrameworkCore;
using Post.Domain.Entities;
using Post.Infrastructure.EntityConfigurations;

namespace Post.Infrastructure.Persistence;

public class PostContext(DbContextOptions<PostContext> options) : DbContext(options)
{
    public required DbSet<PostBase> Posts { get; set; }
    
    public required DbSet<PostActivityLog> PostActivityLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Config column property in database (using Fluent API)
        modelBuilder.ApplyConfiguration(new PostConfiguration());
        modelBuilder.ApplyConfiguration(new PostActivityLogConfiguration());
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var modified = ChangeTracker.Entries()
            .Where(e =>
                e.State is EntityState.Modified or EntityState.Added or EntityState.Deleted);

        foreach (var item in modified)
        {
            switch (item.State)
            {
                case EntityState.Added:
                    if (item.Entity is IDateTracking addedEntity)
                    {
                        addedEntity.CreatedDate = DateTime.UtcNow;
                        item.State = EntityState.Added;
                    }

                    break;

                case EntityState.Modified:
                    Entry(item.Entity).Property("Id").IsModified = false;
                    if (item.Entity is IDateTracking modifiedEntity)
                    {
                        modifiedEntity.LastModifiedDate = DateTime.UtcNow;
                        item.State = EntityState.Modified;
                    }

                    break;
                case EntityState.Detached:
                    break;
                case EntityState.Unchanged:
                    break;
                case EntityState.Deleted:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}