namespace Post.Domain.GrpcClients;

public interface IPostInTagGrpcClient
{
    Task<IEnumerable<Guid>> GetTagIdsByPostIdAsync(Guid postId);

    Task<IEnumerable<Guid>> GetPostIdsInTagAsync(Guid tagId);
}