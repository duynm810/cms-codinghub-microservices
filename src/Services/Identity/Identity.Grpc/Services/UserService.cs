using AutoMapper;
using Grpc.Core;
using Identity.Grpc.Protos;
using Identity.Grpc.Repositories.Interfaces;
using ILogger = Serilog.ILogger;

namespace Identity.Grpc.Services;

public class UserService(IUserRepository userRepository, IMapper mapper, ILogger logger)
    : UserProtoService.UserProtoServiceBase
{
    public override async Task<UserResponse?> GetUserInfo(UserRequest request, ServerCallContext context)
    {
        const string methodName = nameof(GetUserInfo);

        try
        {
            logger.Information("BEGIN {MethodName} - Getting user info by ID: {UserId}", methodName, request.UserId);

            if (!Guid.TryParse(request.UserId, out var userId))
            {
                logger.Warning("{MethodName} - Invalid GUID format: {UserId}", methodName, request.UserId);
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid GUID format"));
            }

            var user = await userRepository.GetUserById(userId);
            if (user == null)
            {
                logger.Warning("{MethodName} - User not found for ID: {UserId}", methodName, request.UserId);
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }

            var data = mapper.Map<UserResponse>(user);

            logger.Information("END {MethodName} - Success: Retrieved User {UserId}", methodName, data.Id);

            return data;
        }
        catch (RpcException rpcEx)
        {
            logger.Error(rpcEx,
                "{MethodName}. gRPC error occurred while getting user info by ID: {UserId}. Message: {ErrorMessage}",
                methodName, request.UserId, rpcEx.Message);
            throw;
        }
        catch (Exception e)
        {
            logger.Error(e,
                "{MethodName}. Error occurred while getting user info by ID: {UserId}. Message: {ErrorMessage}",
                methodName, request.UserId, e.Message);
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while getting user by ID"));
        }
    }


    public override async Task<UsersResponse> GetUsersInfo(UsersRequest request, ServerCallContext context)
    {
        const string methodName = nameof(GetUsersInfo);

        var users = new List<UserResponse>();
        var userIds = new List<Guid>();

        foreach (var id in request.UserIds)
        {
            if (Guid.TryParse(id, out var userId))
            {
                userIds.Add(userId);
            }
            else
            {
                logger.Warning("{MethodName} - Invalid GUID format: {UserId}", methodName, id);
            }
        }

        try
        {
            logger.Information("{MethodName} - Beginning to retrieve users for IDs: {UserIds}", methodName, userIds);

            foreach (var userId in userIds)
            {
                try
                {
                    var user = await userRepository.GetUserById(userId);
                    if (user == null)
                    {
                        logger.Warning("{MethodName} - User not found for ID: {UserId}", methodName, userId);
                        continue;
                    }

                    var userResponse = mapper.Map<UserResponse>(user);
                    users.Add(userResponse);
                }
                catch (Exception e)
                {
                    logger.Error(e,
                        "{MethodName} - Error occurred while getting user info for ID: {UserId}. Message: {ErrorMessage}",
                        methodName, userId, e.Message);
                }
            }

            return new UsersResponse { Users = { users } };
        }
        catch (RpcException rpcEx)
        {
            logger.Error(rpcEx, "{MethodName}. gRPC error occurred while getting user info by IDs. Message: {ErrorMessage}", methodName, rpcEx.Message);
            throw;
        }
        catch (Exception e)
        {
            logger.Error(e, "{MethodName}. Error occurred while getting user info by IDs. Message: {ErrorMessage}", methodName, e.Message);
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while getting users by IDs"));
        }
    }
}