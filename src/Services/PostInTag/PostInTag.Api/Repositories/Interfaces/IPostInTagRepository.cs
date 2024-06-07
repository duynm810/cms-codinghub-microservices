using Contracts.Domains.Repositories;
using PostInTag.Api.Entities;

namespace PostInTag.Api.Repositories.Interfaces;

public interface IPostInTagRepository : IRepositoryCommandBase<PostInTagBase, Guid>
{
    #region CRUD

    Task CreatePostToTag(PostInTagBase postInTagBase);

    Task DeletePostToTag(PostInTagBase postInTagBase);

    #endregion

    #region OTHERS

    Task<IEnumerable<Guid>?> GetPostIdsInTag(Guid tagId);

    #endregion
}