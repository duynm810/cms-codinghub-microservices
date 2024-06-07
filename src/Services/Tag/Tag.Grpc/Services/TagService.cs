using AutoMapper;
using Grpc.Core;
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
        catch (Exception e)
        {
            logger.Error(e,
                "{MethodName}. Error occurred while getting tags by IDs: {TagIds}. Message: {ErrorMessage}",
                methodName, request.Ids, e.Message);
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while getting tags by IDs"));
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
        catch (Exception e)
        {
            logger.Error(e,
                "{MethodName}. Error occurred while getting tags. Message: {ErrorMessage}",
                methodName, e.Message);
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while getting tags"));
        }
    }

    public override async Task<GetTagBySlugResponse> GetTagBySlug(GetTagBySlugRequest request, ServerCallContext context)
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

            var data = mapper.Map<GetTagBySlugResponse>(tag);
            
            logger.Information(
                "END {MethodName} - Success: Retrieved Tag {TagId} - Name: {TagName} - Slug: {TagSlug}",
                methodName, data.Tag.Id, data.Tag.Name, data.Tag.Slug);
            
            return data;
        }
        catch (Exception e)
        {
            logger.Error(e,
                "{MethodName}. Error occurred while getting tag by Slug: {TagSlug}. Message: {ErrorMessage}",
                methodName, request.Slug, e.Message);
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while getting tag by Slug"));
        }
    }
}