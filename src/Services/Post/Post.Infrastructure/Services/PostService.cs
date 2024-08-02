using AutoMapper;
using Post.Domain.Entities;
using Post.Domain.GrpcClients;
using Post.Domain.Services;
using Shared.Dtos.Post;
using Shared.Responses;
using Shared.SeedWorks;

namespace Post.Infrastructure.Services;

public class PostService(ICategoryGrpcClient categoryGrpcClient, IMapper mapper) : IPostService
{
    public async Task<List<PostDto>> EnrichPostsWithCategories(IEnumerable<PostBase> postList, CancellationToken cancellationToken = default)
    {
        var categoryIds = postList.Select(p => p.CategoryId).Distinct().ToList();
        var categories = await categoryGrpcClient.GetCategoriesByIds(categoryIds);

        var postDtos = mapper.Map<List<PostDto>>(postList);
        var categoryDictionary = categories.ToDictionary(c => c.Id, c => c);

        foreach (var postDto in postDtos)
        {
            if (categoryDictionary.TryGetValue(postDto.CategoryId, out var category))
            {
                postDto.Category = category;
            }
        }

        return postDtos;
    }
    
    public async Task<PagedResponse<PostDto>> EnrichPagedPostsWithCategories(PagedResponse<PostBase>? pagedPosts, CancellationToken cancellationToken = default)
    {
        if (pagedPosts is { Items: not null })
        {
            var categoryIds = pagedPosts.Items.Select(p => p.CategoryId).Distinct().ToList();
            var categories = await categoryGrpcClient.GetCategoriesByIds(categoryIds);

            var postDtos = mapper.Map<List<PostDto>>(pagedPosts.Items);
            var categoryDictionary = categories.ToDictionary(c => c.Id, c => c);

            foreach (var postDto in postDtos)
            {
                if (categoryDictionary.TryGetValue(postDto.CategoryId, out var category))
                {
                    postDto.Category = category;
                }
            }

            return new PagedResponse<PostDto>
            {
                Items = postDtos,
                MetaData = pagedPosts.MetaData
            };
        }

        return new PagedResponse<PostDto>()
        {
            Items = [],
            MetaData = new MetaData()
        };
    }
}