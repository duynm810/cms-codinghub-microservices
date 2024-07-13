using AutoMapper;
using Category.Grpc.Protos;
using Google.Protobuf.Collections;
using Post.Grpc.Protos;
using PostInTag.Api.Entities;
using Shared.Dtos.Category;
using Shared.Dtos.Post;
using Shared.Dtos.PostInTag;
using Shared.Dtos.Tag;
using Shared.Requests.PostInTag;
using Tag.Grpc.Protos;

namespace PostInTag.Api;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        ConfigurePostInTagMappings();
        ConfigurePostGrpcMappings();
        ConfigureTagGrpcMappings();
        ConfigureCategoryGrpcMappings();
    }

    private void ConfigurePostInTagMappings()
    {
        CreateMap<CreatePostInTagRequest, PostInTagBase>();
        CreateMap<DeletePostInTagRequest, PostInTagBase>();
    }

    private void ConfigurePostGrpcMappings()
    {
        CreateMap<PostModel, PostInTagDto>().ReverseMap();
        CreateMap<PostModel, PostDto>().ReverseMap();
        CreateMap<PostDto, GetTop10PostsResponse>().ReverseMap();
        CreateMap<GetTop10PostsResponse, IEnumerable<PostDto>>()
            .ConvertUsing(src => src.Posts.Select(p => new PostDto
            {
                Id = Guid.Parse(p.Id),
                Title = p.Title,
                Slug = p.Slug
            }).ToList());
    }

    private void ConfigureTagGrpcMappings()
    {
        CreateMap<TagModel, TagDto>().ReverseMap();
        CreateMap<TagDto, GetTagsResponse>().ReverseMap();
        CreateMap<GetTagsResponse, IEnumerable<TagDto>>()
            .ConvertUsing(src => src.Tags.Select(t => new TagDto
            {
                Id = Guid.Parse(t.Id),
                Name = t.Name,
                Slug = t.Slug
            }).ToList());
    }

    private void ConfigureCategoryGrpcMappings()
    {
        CreateMap<CategoryDto, CategoryModel>()
            .ForMember(dest => dest.Icon, opt => opt.MapFrom(src => src.Icon ?? string.Empty))
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color ?? string.Empty))
            .ReverseMap();

        CreateMap<RepeatedField<CategoryModel>, IEnumerable<CategoryDto>>()
            .ConvertUsing(src => src.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                SeoDescription = c.SeoDescription,
                Icon = c.Icon,
                Color = c.Color
            }).ToList());

        CreateMap<GetCategoriesByIdsResponse, IEnumerable<CategoryDto>>()
            .ConvertUsing(src => src.Categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                SeoDescription = c.SeoDescription,
                Icon = c.Icon,
                Color = c.Color
            }).ToList());
    }
}
