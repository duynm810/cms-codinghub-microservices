using AutoMapper;
using Identity.Infrastructure.Entities;
using Shared.Dtos.Identity.Permission;

namespace Identity.Api;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateOrUpdatePermissionDto, Permission>();
        CreateMap<Permission, PermissionDto>();
    }
}