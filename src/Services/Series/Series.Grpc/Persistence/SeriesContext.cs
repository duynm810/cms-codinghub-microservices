using Microsoft.EntityFrameworkCore;
using Series.Grpc.Entities;

namespace Series.Grpc.Persistence;

public class SeriesContext(DbContextOptions<SeriesContext> options) : DbContext(options)
{
    public required DbSet<SeriesBase> Series { get; set; }
}