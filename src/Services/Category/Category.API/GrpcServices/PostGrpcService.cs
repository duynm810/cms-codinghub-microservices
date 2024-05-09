using Category.API.GrpcServices.Interfaces;
using ILogger = Serilog.ILogger;

namespace Category.API.GrpcServices;

public class PostGrpcService(ILogger logger) : IPostGrpcService
{
    public async Task<bool> HasPostsInCategory(long categoryId)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", nameof(HasPostsInCategory), e);
            return false;
        }
    }
}