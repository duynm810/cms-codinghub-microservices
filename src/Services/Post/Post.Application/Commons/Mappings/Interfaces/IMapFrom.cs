using AutoMapper;

namespace Post.Application.Commons.Mappings.Interfaces;

public interface IMapFrom<T>
{
    void Mapping(Profile profile)
    {
        profile.CreateMap(typeof(T), GetType());
    }
}