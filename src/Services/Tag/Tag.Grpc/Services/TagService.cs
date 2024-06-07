using AutoMapper;
using Grpc.Core;
using Shared.Constants;
using Tag.Grpc.Protos;
using Tag.Grpc.Repositories.Interfaces;
using ILogger = Serilog.ILogger;

namespace Tag.Grpc.Services;

public class TagService(ITagRepository tagRepository, IMapper mapper, ILogger logger)
    : TagProtoService.TagProtoServiceBase
{
    public override async Task<GetTagsByIdsResponse> GetTagsByIds(GetTagsByIdsRequest request,
        ServerCallContext context)
    {
        const string methodName = nameof(GetTagsByIds);

        try
        {
            var tagsIds = request.Ids.Select(Guid.Parse).ToArray();

            logger.Information("{MethodName} - Beginning to retrieve tags for IDs: {TagsIds}", methodName,
                tagsIds);

            var tags = await tagRepository.GetTagsByIds(tagsIds);

            var data = mapper.Map<GetTagsByIdsResponse>(tags);

            return data;
        }
        catch (RpcException rpcEx)
        {
            logger.Error(rpcEx, "{MethodName}. RPC error occurred while getting tags for IDs. : {TagsIds} Status: {StatusCode}, Detail: {Detail}",
                methodName, request.Ids, rpcEx.StatusCode, rpcEx.Status.Detail);
            throw;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            throw new RpcException(new Status(StatusCode.Internal, ErrorMessagesConsts.Common.UnhandledException));
        }
    }

    public override async Task<GetTagsResponse> GetTags(GetTagsRequest request, ServerCallContext context)
    {
        const string methodName = nameof(GetTags);

        try
        {
            logger.Information("{MethodName} - Beginning to retrieve tags", methodName);
            
            var tags = await tagRepository.GetTags();
            
            var data = mapper.Map<GetTagsResponse>(tags);
            
            return data;
        }
        catch (RpcException rpcEx)
        {
            logger.Error(rpcEx, "{MethodName}. RPC error occurred while getting tags. Status: {StatusCode}, Detail: {Detail}",
                methodName, rpcEx.StatusCode, rpcEx.Status.Detail);
            throw;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            throw new RpcException(new Status(StatusCode.Internal, ErrorMessagesConsts.Common.UnhandledException));
        }
    }

    public override async Task<TagModel> GetTagBySlug(GetTagBySlugRequest request, ServerCallContext context)
    {
        const string methodName = nameof(GetTagBySlug);

        try
        {
            logger.Information("BEGIN {MethodName} - Getting tag by Slug: {TagSlug}", methodName,
                request.Slug);

            var tag = await tagRepository.GetTagBySlug(request.Slug);
            if (tag == null)
            {
                logger.Warning("{MethodName} - Tag not found for Slug: {TagSlug}", methodName, request.Slug);
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Tag with slug '{request.Slug}' not found."));
            }

            var data = mapper.Map<TagModel>(tag);
            
            logger.Information(
                "END {MethodName} - Success: Retrieved Tag {TagId} - Name: {TagName} - Slug: {TagSlug}",
                methodName, data.Id, data.Name, data.Slug);
            
            return data;
        }
        catch (RpcException rpcEx)
        {
            logger.Error(rpcEx, "{MethodName}. RPC error occurred while getting tag by Slug: {TagSlug}. Status: {StatusCode}, Detail: {Detail}",
                methodName, request.Slug, rpcEx.StatusCode, rpcEx.Status.Detail);
            throw;
        }
        catch (Exception e)
        {
            logger.Error("{MethodName}. Message: {ErrorMessage}", methodName, e);
            throw new RpcException(new Status(StatusCode.Internal, ErrorMessagesConsts.Common.UnhandledException));
        }
    }
}