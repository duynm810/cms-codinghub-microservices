using AutoMapper;
using Comment.Api.GrpcClients.Interfaces;
using Identity.Grpc.Protos;
using Shared.Dtos.Identity.User;
using ILogger = Serilog.ILogger;

namespace Comment.Api.GrpcClients;

public class IdentityGrpcClient(UserProtoService.UserProtoServiceClient userProtoServiceClient, IMapper mapper, ILogger logger) : IIdentityGrpcClient
{
    public async Task<UserDto?> GetUserInfo(string userId)
    {
        const string methodName = nameof(GetUserInfo);

        try
        {
            var request = new UserRequest() { UserId = userId };
            var result = await userProtoServiceClient.GetUserInfoAsync(request);
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