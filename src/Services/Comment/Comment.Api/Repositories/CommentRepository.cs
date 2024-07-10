using Comment.Api.Entities;
using Comment.Api.Repositories.Interfaces;
using Infrastructure.Domains.Repositories;
using MongoDB.Driver;
using Shared.Settings;

namespace Comment.Api.Repositories;

public class CommentRepository(IMongoClient client, MongoDbSettings settings)
    : MongoRepositoryBase<CommentBase>(client, settings), ICommentRepository
{
    public async Task<List<CommentBase>> GetCommentsByPostId(Guid postId) =>
        await FindAll().Find(x => x.PostId == postId).ToListAsync();

    public async Task<CommentBase?> GetCommentById(string id) => await FindByIdAsync(id);
    
    public async Task<List<CommentBase>> GetLatestComments(int count)
    {
        return await FindAll().Find(_ => true)
            .SortByDescending(c => c.CreatedDate)
            .Limit(count)
            .ToListAsync();
    }

    public async Task<bool> CreateComment(CommentBase comment)
    {
        try
        {
            await CreateAsync(comment);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateLikeCount(string commentId, int increment)
    {
        var filter = Builders<CommentBase>.Filter.Eq(c => c.Id, commentId);
        var update = Builders<CommentBase>.Update.Inc(c => c.Likes, increment);
        var result = await Collection.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> UpdateRepliesCount(string parentId, int increment)
    {
        var filter = Builders<CommentBase>.Filter.Eq(c => c.Id, parentId);
        var update = Builders<CommentBase>.Update.Inc(c => c.RepliesCount, increment);
        var result = await Collection.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }
}