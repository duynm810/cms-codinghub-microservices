using Comment.Api.Entities;
using Comment.Api.GrpcClients.Interfaces;
using Grpc.Core;
using MongoDB.Bson;
using MongoDB.Driver;
using Polly;
using Polly.Retry;
using Shared.Enums;
using Shared.Settings;
using ILogger = Serilog.ILogger;

namespace Comment.Api.Persistence;

public class CommentSeedData
{
    private readonly IPostGrpcClient _postGrpcClient;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly ILogger _logger;

    public CommentSeedData(IPostGrpcClient postGrpcClient, ILogger logger)
    {
        _postGrpcClient = postGrpcClient;
        _logger = logger;

        _retryPolicy = Policy.Handle<RpcException>()
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount) =>
                {
                    _logger.Error($"Retry {retryCount} of EnsureGrpcServicesReadyAsync due to: {exception}.");
                });
    }

    public async Task SeedDataAsync(IMongoClient mongoClient, MongoDbSettings settings)
    {
        var databaseName = settings.DatabaseName;
        var database = mongoClient.GetDatabase(databaseName);

        var commentCollection = database.GetCollection<CommentBase>("Comments");

        if (await commentCollection.EstimatedDocumentCountAsync() > 0)
        {
            return; // Nếu có dữ liệu thì không cần seed thêm
        }
        
        var postIds = await GetPostIdsAsync();
        
        if (postIds == null || postIds.Count == 0)
        {
            throw new Exception("Unable to retrieve posts from Post.GRPC service.");
        }
        
        await commentCollection.InsertManyAsync(GetComments(postIds));
    }

    private static IEnumerable<CommentBase> GetComments(List<Guid> postIds)
    {
        var comments = new List<CommentBase>();
        var random = new Random();

        foreach (var postId in postIds)
        {
            var parentCommentCount = random.Next(1, 3); // 1-2 comments cha

            for (var i = 0; i < parentCommentCount; i++)
            {
                var parentCommentId = ObjectId.GenerateNewId().ToString();
                comments.Add(new CommentBase
                {
                    Id = parentCommentId,
                    UserId = Guid.NewGuid(),
                    PostId = postId,
                    Content = $"Đây là bình luận cha {i + 1} cho bài viết {postId}",
                    ParentId = null,
                    Likes = random.Next(0, 20),
                    RepliesCount = 1,
                    Status = CommentStatusEnum.Approved
                });

                comments.Add(new CommentBase
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    UserId = Guid.NewGuid(),
                    PostId = postId,
                    Content = $"Đây là bình luận con cho bình luận cha {i + 1} của bài viết {postId}",
                    ParentId = parentCommentId,
                    Likes = random.Next(0, 20),
                    RepliesCount = 0,
                    Status = CommentStatusEnum.Approved
                });
            }
        }

        return comments;
    }

    private async Task<List<Guid>> GetPostIdsAsync()
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            _logger.Information("Calling post gRPC service to get posts.");

            var posts = await _postGrpcClient.GetTop10Posts();

            _logger.Information("Successfully retrieved posts from post gRPC service.");

            return posts.Select(p => p.Id).ToList();
        });
    }
}