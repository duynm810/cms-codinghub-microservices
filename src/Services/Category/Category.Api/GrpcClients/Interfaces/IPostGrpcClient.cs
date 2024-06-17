namespace Category.Api.GrpcClients.Interfaces;

public interface IPostGrpcClient
{
    Task<bool> HasPostsInCategory(long categoryId);
}