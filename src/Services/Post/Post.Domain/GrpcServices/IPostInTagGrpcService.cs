namespace Post.Domain.GrpcServices;

public interface IPostInTagGrpcService
{
    Task<IEnumerable<Guid>> GetTagIdsByPostIdAsync(Guid postId);
}