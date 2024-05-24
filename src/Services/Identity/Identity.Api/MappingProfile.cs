using AutoMapper;
using Identity.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Shared.Dtos.Identity.Permission;
using Shared.Dtos.Identity.Role;
using Shared.Dtos.Identity.User;

namespace Identity.Api;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        #region Permission

        CreateMap<CreateOrUpdatePermissionDto, Permission>();
        CreateMap<Permission, PermissionDto>();

        #endregion

        #region Role

        CreateMap<CreateOrUpdateRoleDto, IdentityRole>();
        CreateMap<IdentityRole, RoleDto>();

        #endregion

        #region User

        CreateMap<CreateUserDto, User>();
        CreateMap<UpdateUserDto, User>();
        CreateMap<User, UserDto>();

        #endregion
    }
}