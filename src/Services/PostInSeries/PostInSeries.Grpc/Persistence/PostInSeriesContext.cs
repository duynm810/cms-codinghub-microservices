using Microsoft.EntityFrameworkCore;
using PostInSeries.Grpc.Entities;

namespace PostInSeries.Grpc.Persistence;

public class PostInSeriesContext(DbContextOptions<PostInSeriesContext> options) : DbContext(options)
{
    public required DbSet<PostInSeriesBase> PostInSeries { get; set; }
}