using AutoMapper;
using Identity.Infrastructure.Entities;
using Identity.Infrastructure.Repositories.Interfaces;
using Identity.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Serilog;
using Shared.Constants;
using Shared.Dtos.Identity.User;
using Shared.Responses;
using Shared.Utilities;

namespace Identity.Infrastructure.Services;

public class UserService(IIdentityReposityManager repositoryManager, IMapper mapper, ILogger logger) : IUserService
{
    #region CRUD

    public async Task<ApiResult<UserDto?>> CreateUser(CreateUserDto model)
    {
        var result = new ApiResult<UserDto?>();
        const string methodName = nameof(CreateUser);

        try
        {
            logger.Information("BEGIN {MethodName} - Creating user with username: {UserName}", methodName, model.UserName);

            var user = mapper.Map<User>(model);
            await repositoryManager.Users.CreateUser(user, model.Password);

            var data = mapper.Map<UserDto>(user);
            result.Success(data);

            logger.Information("END {MethodName} - User created successfully with ID {UserId}", methodName, user.Id);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<bool>> UpdateUser(Guid userId, UpdateUserDto model)
    {
        var result = new ApiResult<bool>();
        const string methodName = nameof(UpdateUser);

        try
        {
            logger.Information("BEGIN {MethodName} - Updating user with ID: {UserId}", methodName, userId);

            var user = await repositoryManager.Users.GetUserById(userId);
            if (user == null)
            {
                result.Messages.Add(ErrorMessagesConsts.Identity.User.UserNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            mapper.Map(model, user);
            var updateResult = await repositoryManager.Users.UpdateUser(user);
            if (!updateResult)
            {
                result.Messages.Add(ErrorMessagesConsts.Identity.User.UserUpdateFailed);
                result.Failure(StatusCodes.Status400BadRequest, result.Messages);
                return result;
            }

            result.Success(true);

            logger.Information("END {MethodName} - User updated successfully with ID {UserId}", methodName, userId);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<bool>> DeleteUser(Guid userId)
    {
        var result = new ApiResult<bool>();
        const string methodName = nameof(DeleteUser);

        try
        {
            logger.Information("BEGIN {MethodName} - Deleting user with ID: {UserId}", methodName, userId);

            var user = await repositoryManager.Users.GetUserById(userId);
            if (user == null)
            {
                result.Messages.Add(ErrorMessagesConsts.Identity.User.UserNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var deleteResult = await repositoryManager.Users.DeleteUser(user);
            if (!deleteResult)
            {
                result.Messages.Add(ErrorMessagesConsts.Identity.User.UserDeleteFailed);
                result.Failure(StatusCodes.Status400BadRequest, result.Messages);
                return result;
            }

            result.Success(true);

            logger.Information("END {MethodName} - User deleted successfully with ID {UserId}", methodName, userId);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<IEnumerable<UserDto>>> GetUsers()
    {
        var result = new ApiResult<IEnumerable<UserDto>>();
        const string methodName = nameof(GetUsers);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving all users", methodName);

            var users = await repositoryManager.Users.GetUsers();
            var data = mapper.Map<IEnumerable<UserDto>>(users);

            result.Success(data);

            logger.Information("END {MethodName} - Successfully retrieved users", methodName);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    public async Task<ApiResult<UserDto>> GetUserById(Guid userId)
    {
        var result = new ApiResult<UserDto>();
        const string methodName = nameof(GetUserById);

        try
        {
            logger.Information("BEGIN {MethodName} - Retrieving user with ID: {UserId}", methodName, userId);

            var user = await repositoryManager.Users.GetUserById(userId);
            if (user == null)
            {
                result.Messages.Add(ErrorMessagesConsts.Identity.User.UserNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var data = mapper.Map<UserDto>(user);
            result.Success(data);

            logger.Information("END {MethodName} - Successfully retrieved user with ID {UserId}", methodName, userId);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }
    
    #endregion

    #region OTHERS

    public async Task<ApiResult<bool>> ChangePassword(Guid userId, ChangePasswordUserDto model)
    {
        var result = new ApiResult<bool>();
        const string methodName = nameof(ChangePassword);

        try
        {
            logger.Information("BEGIN {MethodName} - Changing password for user with ID: {UserId}", methodName, userId);

            var user = await repositoryManager.Users.GetUserById(userId);
            if (user == null)
            {
                result.Messages.Add(ErrorMessagesConsts.Identity.User.UserNotFound);
                result.Failure(StatusCodes.Status404NotFound, result.Messages);
                return result;
            }

            var changePasswordResult = await repositoryManager.Users.ChangePassword(user, model.CurrentPassword, model.NewPassword);
            if (!changePasswordResult)
            {
                result.Messages.Add(ErrorMessagesConsts.Identity.User.UserChangePasswordFailed);
                result.Failure(StatusCodes.Status400BadRequest, result.Messages);
                return result;
            }

            result.Success(true);
            logger.Information("END {MethodName} - Password changed successfully for user with ID {UserId}", methodName, userId);
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            result.Messages.AddRange(e.GetExceptionList());
            result.Failure(StatusCodes.Status500InternalServerError, result.Messages);
        }

        return result;
    }

    #endregion
}