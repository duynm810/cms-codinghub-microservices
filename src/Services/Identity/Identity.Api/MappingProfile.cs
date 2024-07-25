using AutoMapper;
using Identity.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Shared.Dtos.Identity.Permission;
using Shared.Dtos.Identity.Role;
using Shared.Dtos.Identity.User;
using Shared.Extensions;
using Shared.Requests.Identity.Permission;
using Shared.Requests.Identity.Role;
using Shared.Requests.Identity.User;

namespace Identity.Api;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        #region PERMISSION

        CreateMap<CreateOrUpdatePermissionRequest, Permission>();
        CreateMap<Permission, PermissionDto>();

        #endregion

        #region ROLE

        CreateMap<CreateOrUpdateRoleRequest, IdentityRole>();
        CreateMap<IdentityRole, RoleDto>();

        #endregion

        #region USER

        CreateMap<CreateUserRequest, User>();
        CreateMap<UpdateUserRequest, User>();
        CreateMap<UpdateAvatarRequest, User>().IgnoreAllNonExisting();
        CreateMap<User, UserDto>();

        #endregion
    }
}