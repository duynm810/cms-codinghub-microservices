using Contracts.Domains.Interfaces;
using Microsoft.EntityFrameworkCore;
using Tag.Grpc.Entities;

namespace Tag.Grpc.Persistence;

public class TagContext(DbContextOptions<TagContext> options) : DbContext(options)
{
    public required DbSet<TagBase> Tags { get; set; }
}