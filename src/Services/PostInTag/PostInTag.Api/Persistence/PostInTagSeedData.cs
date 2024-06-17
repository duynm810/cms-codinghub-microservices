using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;
using PostInTag.Api.Entities;
using PostInTag.Api.GrpcClients.Interfaces;
using ILogger = Serilog.ILogger;

namespace PostInTag.Api.Persistence;

public static class PostInTagSeedData
{
    public static IHost SeedData(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var postInTagContext = scope.ServiceProvider.GetRequiredService<PostInTagContext>();
        var tagGrpcService = scope.ServiceProvider.GetRequiredService<ITagGrpcClient>();
        var postGrpcService = scope.ServiceProvider.GetRequiredService<IPostGrpcClient>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
        
        postInTagContext.Database.MigrateAsync().GetAwaiter().GetResult();

        var seedData = new Seeder(postInTagContext, tagGrpcService, postGrpcService, logger);
        seedData.SeedAsync().GetAwaiter().GetResult();

        return host;
    }

    private class Seeder
    {
        private readonly PostInTagContext _context;
        private readonly ITagGrpcClient _tagGrpcClient;
        private readonly IPostGrpcClient _postGrpcClient;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly ILogger _logger;
        
        private static readonly Random Random = new();
        private IEnumerable<Guid> _tags = new List<Guid>();
        private IEnumerable<Guid> _posts = new List<Guid>();

        public Seeder(PostInTagContext context, ITagGrpcClient tagGrpcClient, IPostGrpcClient postGrpcClient, ILogger logger)
        {
            _context = context;
            _tagGrpcClient = tagGrpcClient;
            _postGrpcClient = postGrpcClient;
            _logger = logger;

            _retryPolicy = Policy.Handle<RpcException>()
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount) =>
                    {
                        _logger.Error($"Retry {retryCount} of EnsureGrpcServicesReadyAsync due to: {exception}.");
                    });
        }
        
        public async Task SeedAsync()
        {
            try
            {
                await TrySeedAsync();
                await _context.SaveChangesAsync();

                _logger.Information("Data seeding completed successfully.");
            }
            catch (Exception e)
            {
                _logger.Error(e, "An error occurred while seeding the database.");
                throw;
            }
        }
        
        private async Task TrySeedAsync()
        {
            if (!_context.PostInTag.Any())
            {
                _logger.Information("Starting EnsureGrpcServicesReadyAsync.");

                await EnsureGrpcServicesReadyAsync();

                _logger.Information("gRPC services are ready. Starting to seed data.");

                var postInTagData = _posts.Select(postId => new PostInTagBase
                {
                    Id = Guid.NewGuid(),
                    TagId = GetRandomTag(),
                    PostId = postId,
                    SortOrder = Random.Next(1, 100)
                }).ToList();

                await _context.PostInTag.AddRangeAsync(postInTagData);
            }
        }

        private async Task EnsureGrpcServicesReadyAsync()
        {
            var tags = await _retryPolicy.ExecuteAsync(async () =>
            {
                _logger.Information("Calling tag gRPC service to get tags.");

                var tags = await _tagGrpcClient.GetTags();

                _logger.Information("Successfully retrieved tags from tag gRPC service.");

                return tags;
            });

            _tags = tags.Select(t => t.Id);

            _posts = await _retryPolicy.ExecuteAsync(async () =>
            {
                _logger.Information("Calling post gRPC service to get posts.");

                var posts = await _postGrpcClient.GetTop10Posts();

                _logger.Information("Successfully retrieved posts from post gRPC service.");

                return posts.Select(p => p.Id);
            });
        }

        private Guid GetRandomTag()
        {
            var tagList = _tags.ToList();
            return tagList[Random.Next(tagList.Count)];
        }
    }
}