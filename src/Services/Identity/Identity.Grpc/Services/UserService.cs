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

         if (!Guid.TryParse(request.UserId, out var userId))
         {
            logger.Warning("{MethodName} - Invalid GUID format: {UserId}", methodName, request.UserId);
            return new UserResponse(); // Return response if Guid invalid (Trả về response rỗng nếu GUID không hợp lệ)
         }
         
         var user = await userRepository.GetUserById(userId);
         if (user == null)
         {
            logger.Warning("{MethodName} - User not found for ID: {UserId}", methodName, request.UserId);
            return new UserResponse { Id = string.Empty }; // Make sure that Id is not null (Đảm bảo rằng Id không null)
         }

         var data = mapper.Map<UserResponse>(user);

         logger.Information("END {MethodName} - Success: Retrieved User {UserId}", methodName, data.Id);

         return data;
      }
      catch (RpcException e)
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
      catch (RpcException e)
      {
         logger.Error(e, "{MethodName}. Error occurred while getting user info by IDs: {UserId}. Message: {ErrorMessage}", methodName, userIds, e.Message);
         throw new RpcException(new Status(StatusCode.Internal, "An occurred while getting user by IDs"));
      }
   }
}