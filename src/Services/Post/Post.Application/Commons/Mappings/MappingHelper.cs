using AutoMapper;
using Post.Application.Commons.Mappings.Interfaces;
using Post.Application.Commons.Models;
using Post.Domain.Entities;
using Shared.Dtos.Category;
using Shared.Responses;

namespace Post.Application.Commons.Mappings;

public class MappingHelper(IMapper mapper) : IMappingHelper
{
    private void MapCategoryToPosts(IEnumerable<PostModel> postModels, CategoryDto category)
    {
        foreach (var post in postModels)
        {
            mapper.Map(category, post);
        }
    }

    public List<PostModel> MapPostsWithCategories(IEnumerable<PostBase>? postBases, IEnumerable<CategoryDto> categories)
    {
        if (postBases == null)
        {
            return [];
        }
        
        var postModels = mapper.Map<List<PostModel>>(postBases);
        var categoryDictionary = categories.ToDictionary(c => c.Id, c => c);
        
        foreach (var post in postModels)
        {
            if (categoryDictionary.TryGetValue(post.CategoryId, out var category))
            {
                mapper.Map(category, post);
            }
        }
        
        return postModels;
    }
    
    public PagedResponse<PostModel> MapPostsWithCategories(PagedResponse<PostBase> pagedPosts, IEnumerable<CategoryDto> categories)
    {
        var postModels = MapPostsWithCategories(pagedPosts.Items, categories);
        
        return new PagedResponse<PostModel>
        {
            Items = postModels,
            MetaData = pagedPosts.MetaData
        };
    }
    
    public PagedResponse<PostModel> MapPostsWithCategory(PagedResponse<PostBase> pagedPosts, CategoryDto category)
    {
        var postModels = mapper.Map<List<PostModel>>(pagedPosts.Items);
        MapCategoryToPosts(postModels, category);
        
        return new PagedResponse<PostModel>
        {
            Items = postModels,
            MetaData = pagedPosts.MetaData
        };
    }
    
    public PostModel MapPostWithCategory(PostBase post, CategoryDto category)
    {
        var postModel = mapper.Map<PostModel>(post);
        mapper.Map(category, postModel);
        return postModel;
    }
}
