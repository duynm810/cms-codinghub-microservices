namespace Post.Domain.GrpcClients;

public interface IPostInTagGrpcClient
{
    Task<IEnumerable<Guid>> GetTagIdsByPostIdAsync(Guid postId);
}