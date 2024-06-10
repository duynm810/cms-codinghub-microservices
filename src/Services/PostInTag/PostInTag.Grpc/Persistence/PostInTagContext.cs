using Contracts.Domains.Interfaces;
using Microsoft.EntityFrameworkCore;
using PostInTag.Grpc.Entities;

namespace PostInTag.Grpc.Persistence;

public class PostInTagContext(DbContextOptions<PostInTagContext> options) : DbContext(options)
{
    public required DbSet<PostInTagBase> PostInTag { get; set; }
}