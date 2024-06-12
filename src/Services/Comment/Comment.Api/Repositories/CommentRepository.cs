using Comment.Api.Entities;
using Comment.Api.Repositories.Interfaces;
using Infrastructure.Domains.Repositories;
using MongoDB.Driver;
using Shared.Settings;

namespace Comment.Api.Repositories;

public class CommentRepository(IMongoClient client, MongoDbSettings settings)
    : MongoRepositoryBase<CommentBase>(client, settings), ICommentRepository
{
    
    public async Task<IEnumerable<CommentBase>> GetCommentsByPostId(Guid postId) => 
        await FindAll().Find(x => x.PostId == postId).ToListAsync();

    public async Task CreateComment(CommentBase comment) => await CreateAsync(comment);
}