using AutoMapper;
using Grpc.Core;
using Identity.Grpc.Protos;
using Post.Domain.GrpcClients;
using Serilog;
using Shared.Constants;
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
            var request = new UserRequest { UserId = userId.ToString("D") };

            var result = await userProtoServiceClient.GetUserInfoAsync(request);
            if (result == null)
            {
                logger.Warning("{MethodName}: No user found with id {Id}", methodName, userId);
                return null;
            }

            var data = mapper.Map<UserDto>(result);
            return data;
        }
        catch (RpcException rpcEx)
        {
            logger.Error("{MethodName}: gRPC error occurred while getting user info by ID: {Id}. StatusCode: {StatusCode}. Message: {ErrorMessage}", methodName, userId, rpcEx.StatusCode, rpcEx.Message);
            throw;
        }
        catch (Exception e)
        {
            logger.Error(e, "{MethodName}: Unexpected error occurred while getting user info by ID: {Id}. Message: {ErrorMessage}", methodName, userId, e.Message);
            throw;
        }
    }

    public async Task<UserDto?> GetUserInfoByUserName(string userName)
    {
        const string methodName = nameof(GetUserInfoByUserName);

        try
        {
            var request = new UserNameRequest { UserName = userName };
            
            var result = await userProtoServiceClient.GetUserInfoByUserNameAsync(request);
            if (result == null)
            {
                logger.Warning("{MethodName}: No user found with username {Username}", methodName, userName);
                return null;
            }
            
            var data = mapper.Map<UserDto>(result);
            return data;
        }
        catch (RpcException rpcEx)
        {
            logger.Error("{MethodName}: gRPC error occurred while getting user info by username: {Username}. StatusCode: {StatusCode}. Message: {ErrorMessage}", methodName, userName, rpcEx.StatusCode, rpcEx.Message);
            return null;
        }
        catch (Exception e)
        {
            logger.Error(e, "{MethodName}: Unexpected error occurred while getting user info by username: {Username}. Message: {ErrorMessage}", methodName, userName, e.Message);
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while getting user by username"));
        }
    }
}