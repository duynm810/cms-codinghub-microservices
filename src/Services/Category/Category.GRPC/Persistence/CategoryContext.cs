using Category.GRPC.Entities;
using Microsoft.EntityFrameworkCore;

namespace Category.GRPC.Persistence;

public class CategoryContext(DbContextOptions<CategoryContext> options) : DbContext(options)
{
    public required DbSet<CategoryBase> Categories { get; set; }
}