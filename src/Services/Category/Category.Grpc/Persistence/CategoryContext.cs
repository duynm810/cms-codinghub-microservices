using Category.Grpc.Entities;
using Microsoft.EntityFrameworkCore;

namespace Category.Grpc.Persistence;

public class CategoryContext(DbContextOptions<CategoryContext> options) : DbContext(options)
{
    public required DbSet<CategoryBase> Categories { get; set; }
}