namespace Post.Domain.Interfaces;

public interface IDatabaseSeeder
{
    Task InitialiseAsync();

    Task SeedAsync();
}