using AutoMapper;
using Grpc.Core;
using Identity.Grpc.Protos;
using Identity.Grpc.Repositories.Interfaces;
using ILogger = Serilog.ILogger;

namespace Identity.Grpc.Services;

public class UserService(IUserRepository userRepository, IMapper mapper, ILogger logger) : UserProtoService.UserProtoServiceBase
{
   public override async Task<UserResponse?> GetUserInfo(UserRequest request, ServerCallContext context)
   {
      const string methodName = nameof(GetUserInfo);
      
      try
      {
         logger.Information("BEGIN {MethodName} - Getting user info by ID: {UserId}", methodName, request.UserId);

         var user = await userRepository.GetUserById(Guid.Parse(request.UserId));
         if (user == null)
         {
            logger.Warning("{MethodName} - User not found for ID: {UserId}", methodName, request.UserId);
            return null;
         }

         var data = mapper.Map<UserResponse>(user);

         logger.Information("END {MethodName} - Success: Retrieved User {UserId}", methodName, data.UserId);

         return data;
      }
      catch (Exception e)
      {
         logger.Error(e, "{MethodName}. Error occurred while getting user by ID: {UserId}. Message: {ErrorMessage}", methodName, request.UserId, e.Message);
         throw new RpcException(new Status(StatusCode.Internal, "An occurred while getting user by ID"));
      }
   }
}