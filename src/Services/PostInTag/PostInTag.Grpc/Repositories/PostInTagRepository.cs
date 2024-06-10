using Contracts.Domains.Repositories;
using Infrastructure.Domains.Repositories;
using Microsoft.EntityFrameworkCore;
using PostInTag.Grpc.Entities;
using PostInTag.Grpc.Persistence;
using PostInTag.Grpc.Repositories.Interfaces;

namespace PostInTag.Grpc.Repositories;

public class PostInTagRepository(PostInTagContext dbContext, IUnitOfWork<PostInTagContext> unitOfWork)
    : RepositoryCommandBase<PostInTagBase, Guid, PostInTagContext>(dbContext, unitOfWork), IPostInTagRepository
{
}