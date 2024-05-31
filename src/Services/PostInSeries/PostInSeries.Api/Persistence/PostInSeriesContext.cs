using Contracts.Domains.Interfaces;
using Microsoft.EntityFrameworkCore;
using PostInSeries.Api.Entities;

namespace PostInSeries.Api.Persistence;

public class PostInSeriesContext(DbContextOptions<PostInSeriesContext> options) : DbContext(options)
{
    public required DbSet<PostInSeriesBase> PostInSeries { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
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