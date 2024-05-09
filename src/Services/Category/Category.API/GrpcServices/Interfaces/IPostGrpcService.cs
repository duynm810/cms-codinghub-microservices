namespace Category.API.GrpcServices.Interfaces;

public interface IPostGrpcService
{
    Task<bool> HasPostsInCategory(long categoryId);
}