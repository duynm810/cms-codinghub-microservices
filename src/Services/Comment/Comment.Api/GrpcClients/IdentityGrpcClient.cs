using AutoMapper;
using Comment.Api.GrpcClients.Interfaces;
using Grpc.Core;
using Identity.Grpc.Protos;
using Shared.Constants;
using Shared.Dtos.Identity.User;
using ILogger = Serilog.ILogger;

namespace Comment.Api.GrpcClients;

public class IdentityGrpcClient(
    UserProtoService.UserProtoServiceClient userProtoServiceClient,
    IMapper mapper,
    ILogger logger) : IIdentityGrpcClient
{
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
        catch (RpcException rpcEx)
        {
            logger.Error("{MethodName}: gRPC error occurred while getting users info by IDs: {Id}. StatusCode: {StatusCode}. Message: {ErrorMessage}", methodName, userIds, rpcEx.StatusCode, rpcEx.Message);
            return [];
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            throw new RpcException(new Status(StatusCode.Internal, ErrorMessagesConsts.Common.UnhandledException));
        }
    }
}