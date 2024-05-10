namespace Category.Api.GrpcServices.Interfaces;

public interface IPostGrpcService
{
    Task<bool> HasPostsInCategory(long categoryId);
}