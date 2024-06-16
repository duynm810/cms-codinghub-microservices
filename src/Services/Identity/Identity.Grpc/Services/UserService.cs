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
         logger.Error(e, "{MethodName}. Error occurred while getting user info by ID: {UserId}. Message: {ErrorMessage}", methodName, request.UserId, e.Message);
         throw new RpcException(new Status(StatusCode.Internal, "An occurred while getting user by ID"));
      }
   }

   public override async Task<UsersResponse> GetUsersInfo(UsersRequest request, ServerCallContext context)
   {
      const string methodName = nameof(GetUsersInfo);

      var userIds = request.UserIds.Select(Guid.Parse).ToList();

      try
      {
         var users = new List<UserResponse>();
          
         logger.Information("{MethodName} - Beginning to retrieve users for IDs: {UserIds}", methodName, userIds);
         
         foreach (var userId in userIds)
         {
            var user = await userRepository.GetUserById(userId);
            if (user == null)
            {
               continue;
            }
             
            var userResponse = mapper.Map<UserResponse>(user);
            users.Add(userResponse);
         }

         return new UsersResponse { Users = { users } };
      }
      catch (Exception e)
      {
         logger.Error(e, "{MethodName}. Error occurred while getting user info by IDs: {UserId}. Message: {ErrorMessage}", methodName, userIds, e.Message);
         throw new RpcException(new Status(StatusCode.Internal, "An occurred while getting user by IDs"));
      }
   }
}