using Shared.Dtos.Identity.User;
using Shared.Requests.Identity.User;
using Shared.Responses;

namespace Identity.Infrastructure.Services.Interfaces;

public interface IUserService
{
    #region CRUD

    Task<ApiResult<UserDto>> CreateUser(CreateUserRequest request);

    Task<ApiResult<bool>> UpdateUser(Guid userId, UpdateUserRequest request);

    Task<ApiResult<bool>> DeleteUser(Guid userId);

    Task<ApiResult<IEnumerable<UserDto>>> GetUsers();

    Task<ApiResult<UserDto>> GetUserById(string userId);
    
    Task<ApiResult<UserDto>> GetUserByName(string? name);

    #endregion

    #region OTHERS
    
    Task<ApiResult<bool>> ChangePassword(Guid userId, ChangePasswordUserRequest request);
    
    Task<ApiResult<bool>> UpdateAvatar(Guid userId, UpdateAvatarRequest request);

    #endregion
}