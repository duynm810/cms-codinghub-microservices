using AutoMapper;
using Identity.Grpc.Protos;
using Post.Domain.GrpcClients;
using Serilog;
using Shared.Dtos.Identity.User;

namespace Post.Infrastructure.GrpcClients;

public class IdentityGrpcClient(
    UserProtoService.UserProtoServiceClient userProtoServiceClient,
    IMapper mapper,
    ILogger logger) : IIdentityGrpcClient
{
    public async Task<UserDto?> GetUserInfo(Guid userId)
    {
        const string methodName = nameof(GetUserInfo);

        try
        {
            var request = new UserRequest { UserId = userId.ToString() };

            var result = await userProtoServiceClient.GetUserInfoAsync(request);
            if (result == null)
            {
                logger.Warning("{MethodName}: No user found with id {Id}", methodName, userId);
                return null;
            }

            var data = mapper.Map<UserDto>(result);

            return data;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            throw;
        }
    }
}