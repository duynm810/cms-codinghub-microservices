using AutoMapper;
using Comment.Api.GrpcClients.Interfaces;
using Identity.Grpc.Protos;
using Shared.Dtos.Identity.User;
using ILogger = Serilog.ILogger;

namespace Comment.Api.GrpcClients;

public class IdentityGrpcClient(
    UserProtoService.UserProtoServiceClient userProtoServiceClient,
    IMapper mapper,
    ILogger logger) : IIdentityGrpcClient
{
    public async Task<UserDto?> GetUserInfo(string userId)
    {
        const string methodName = nameof(GetUserInfo);

        try
        {
            var request = new UserRequest() { UserId = userId };
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

    public async Task<List<UserDto>> GetUsersInfo(IEnumerable<Guid> userIds)
    {
        const string methodName = nameof(GetUsersInfo);

        try
        {
            var idList = userIds as Guid[] ?? userIds.ToArray();

            var request = new UsersRequest();
            request.UserIds.AddRange(idList.Select(id => id.ToString()));
            
            var result = await userProtoServiceClient.GetUsersInfoAsync(request);
            if (result == null)
            {
                logger.Warning("{MethodName}: No users info not found", methodName);
                return [];
            }
            
            var data = mapper.Map<List<UserDto>>(result);
            return data;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            throw;
        }
    }
}