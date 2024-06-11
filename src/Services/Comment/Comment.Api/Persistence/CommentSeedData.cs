using Comment.Api.Entities;
using MongoDB.Driver;
using Shared.Enums;
using Shared.Settings;

namespace Comment.Api.Persistence;

public class CommentSeedData
{
    public async Task SeedDataAsync(IMongoClient mongoClient, MongoDbSettings settings)
    {
        var databaseName = settings.DatabaseName;
        var database = mongoClient.GetDatabase(databaseName);

        var commentCollection = database.GetCollection<CommentBase>("Comments");

        if (await commentCollection.EstimatedDocumentCountAsync() > 0)
        {
            return; // Nếu có dữ liệu thì không cần seed thêm
        }

        await commentCollection.InsertManyAsync(GetComments());
    }
    
    private IEnumerable<CommentBase> GetComments()
    {
        var postId1 = Guid.NewGuid();
        var postId2 = Guid.NewGuid();
        var postId3 = Guid.NewGuid();
        
        return new List<CommentBase>
        {
            new()
            {
                UserId = Guid.NewGuid(),
                PostId = postId1,
                Content = "Đây là một bài viết tuyệt vời!",
                ParentId = null,
                Likes = 10,
                RepliesCount = 2,
                Status = CommentStatusEnum.Approved
            },
            new()
            {
                UserId = Guid.NewGuid(),
                PostId = postId1,
                Content = "Tôi hoàn toàn đồng ý với điều này.",
                ParentId = null,
                Likes = 5,
                RepliesCount = 0,
                Status = CommentStatusEnum.Pending
            },
            new()
            {
                UserId = Guid.NewGuid(),
                PostId = postId2,
                Content = "Quan điểm thú vị.",
                ParentId = null,
                Likes = 3,
                RepliesCount = 1,
                Status = CommentStatusEnum.Approved
            },
            new()
            {
                UserId = Guid.NewGuid(),
                PostId = postId3,
                Content = "Bạn có thể cung cấp thêm chi tiết không?",
                ParentId = null,
                Likes = 2,
                RepliesCount = 1,
                Status = CommentStatusEnum.Approved
            },
            new()
            {
                UserId = Guid.NewGuid(),
                PostId = postId2,
                Content = "Cảm ơn bạn đã chia sẻ!",
                ParentId = null,
                Likes = 8,
                RepliesCount = 3,
                Status = CommentStatusEnum.Approved
            },
            new()
            {
                UserId = Guid.NewGuid(),
                PostId = postId3,
                Content = "Tôi không đồng ý với quan điểm của bạn.",
                ParentId = null,
                Likes = 1,
                RepliesCount = 1,
                Status = CommentStatusEnum.Approved
            },
            new()
            {
                UserId = Guid.NewGuid(),
                PostId = postId1,
                Content = "Đây là spam!",
                ParentId = null,
                Likes = 0,
                RepliesCount = 0,
                Status = CommentStatusEnum.Spam
            },
            new()
            {
                UserId = Guid.NewGuid(),
                PostId = postId2,
                Content = "Những hiểu biết tuyệt vời!",
                ParentId = null,
                Likes = 12,
                RepliesCount = 4,
                Status = CommentStatusEnum.Approved
            }
        };
    }
}