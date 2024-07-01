namespace Tag.Api.GrpcClients.Interfaces;

public interface IPostInTagGrpcClient
{
    Task<IEnumerable<Guid>> GetTagIdsByPostIdAsync(Guid postId);
}