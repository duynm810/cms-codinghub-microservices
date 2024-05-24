using Shared.Dtos.Identity.User;
using Shared.Responses;

namespace Identity.Infrastructure.Services.Interfaces;

public interface IUserService
{
    #region CRUD

    Task<ApiResult<UserDto?>> CreateUser(CreateUserDto model);

    Task<ApiResult<bool>> UpdateUser(Guid userId, UpdateUserDto model);

    Task<ApiResult<bool>> DeleteUser(Guid userId);

    Task<ApiResult<IEnumerable<UserDto>>> GetUsers();

    Task<ApiResult<UserDto>> GetUserById(Guid userId);

    #endregion

    #region OTHERS

    Task<ApiResult<bool>> ChangePassword(Guid userId, ChangePasswordUserDto model);

    #endregion
}