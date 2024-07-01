using Contracts.Domains.Repositories;
using PostInTag.Api.Entities;

namespace PostInTag.Api.Repositories.Interfaces;

public interface IPostInTagRepository : IRepositoryCommandBase<PostInTagBase, Guid>
{
    #region CRUD

    Task CreatePostToTag(PostInTagBase postInTagBase);

    Task DeletePostToTag(PostInTagBase postInTagBase);

    Task<PostInTagBase?> GetPostInTag(Guid postId, Guid tagId);

    #endregion
}