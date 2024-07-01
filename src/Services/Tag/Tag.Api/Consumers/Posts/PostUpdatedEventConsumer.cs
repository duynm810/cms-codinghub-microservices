using AutoMapper;
using EventBus.IntegrationEvents;
using EventBus.IntegrationEvents.Interfaces;
using MassTransit;
using Microsoft.Extensions.Options;
using Shared.Constants;
using Shared.Dtos.Tag;
using Shared.Settings;
using Tag.Api.Entities;
using Tag.Api.GrpcClients.Interfaces;
using Tag.Api.Repositories.Interfaces;
using ILogger = Serilog.ILogger;

namespace Tag.Api.Consumers.Posts;

public class PostUpdatedEventConsumer(
    IPublishEndpoint publishEndpoint,
    ITagRepository tagRepository,
    IPostInTagGrpcClient postInTagGrpcClient,
    IOptions<EventBusSettings> eventBusSettings,
    IMapper mapper,
    ILogger logger) : IConsumer<IPostUpdatedEvent>
{
    private readonly EventBusSettings _eventBusSettings = eventBusSettings.Value;

    public async Task Consume(ConsumeContext<IPostUpdatedEvent> context)
    {
        var message = context.Message;

        const string methodName = nameof(Consume);
        const string className = nameof(PostUpdatedEventConsumer);

        logger.Information("BEGIN processing {ClassName} - PostId: {PostId}", className, message.PostId);

        await using var transaction = await tagRepository.BeginTransactionAsync();

        try
        {
            // Get existing tags for the post using gRPC (Lấy danh sách các tags hiện tại của bài viết bằng gRPC)
            var existingTagIds = (await postInTagGrpcClient.GetTagIdsByPostIdAsync(message.PostId)).ToList();

            var newTagIds = new List<Guid>();
            var processedTagIds = new List<Guid>();

            var newTags = message.RawTags.Where(t => !t.IsExisting).ToList();
            var existingTags = message.RawTags.Where(t => t.IsExisting).ToList();

            var newTagTasks = newTags.Select(CreateNewTag).ToList();
            var existingTagTasks = existingTags.Select(rawTag =>
            {
                var tagId = Guid.Parse(rawTag.Id);
                processedTagIds.Add(tagId);
                if (!existingTagIds.Contains(tagId))
                {
                    return UpdateExistingTag(tagId);
                }

                return Task.CompletedTask;
            }).ToList();

            await Task.WhenAll(newTagTasks);
            newTagIds.AddRange(newTagTasks.Select(t => t.Result));

            await Task.WhenAll(existingTagTasks);

            // Find tags to remove (Tìm các tags cần xóa)
            var tagsToRemove = existingTagIds.Except(processedTagIds).ToList();

            // Add new tags only if they are not in existing tags (Chỉ thêm các tag mới nếu chúng không nằm trong existing tags)
            newTagIds.AddRange(processedTagIds.Where(tagId => !existingTagIds.Contains(tagId)));

            // Decrease UsageCount for removed tags (Giảm UsageCount cho các tags cần xóa)
            var decreaseTasks = tagsToRemove.Select(DecreaseTagUsageCount).ToList();
            await Task.WhenAll(decreaseTasks);

            await transaction.CommitAsync();

            // Publish event for processed tags (Phát hành sự kiện cho các tags đã được xử lý)
            await PublishPostTagsUpdatedEvent(message.PostId, newTagIds, tagsToRemove);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.Error(e,
                "ERROR while processing {MethodName} - PostId: {PostId}. Error: {ErrorMessage}", methodName,
                message.PostId, e.Message);
            throw;
        }
    }

    #region HELPERS

    private async Task PublishPostTagsUpdatedEvent(Guid postId, List<Guid> newTagIds, List<Guid> tagsToRemove)
    {
        const string methodName = nameof(PublishPostTagsUpdatedEvent);

        var serviceName = _eventBusSettings.ServiceName;

        logger.Information("BEGIN Publish {MethodName} - PostId: {PostId}, SourceService: {SourceService}", methodName,
            postId, serviceName);

        try
        {
            var postTagsUpdatedEvent = new PostTagsUpdatedEvent(_eventBusSettings.ServiceName)
            {
                PostId = postId,
                NewTagIds = newTagIds,
                TagsToRemove = tagsToRemove
            };

            await publishEndpoint.Publish<IPostTagsUpdatedEvent>(postTagsUpdatedEvent);

            logger.Information(
                "END Publish {MethodName} successfully - PostId: {PostId}, SourceService: {SourceService}", methodName,
                postId, serviceName);
        }
        catch (Exception e)
        {
            logger.Error(e,
                "ERROR while publishing {MethodName} - PostId: {PostId}. Error: {ErrorMessage}",
                methodName, postId, e.Message);
            throw;
        }
    }

    private async Task<Guid> CreateNewTag(RawTagDto rawTag)
    {
        const string className = nameof(PostUpdatedEventConsumer);
        const string methodName = nameof(CreateNewTag);

        try
        {
            var tagDto = new CreateTagDto
            {
                Name = rawTag.Name,
                Slug = rawTag.Slug
            };

            var tag = mapper.Map<TagBase>(tagDto);

            // Set initial usage count to 1 (Thiết lập usage count ban đầu là 1)
            tag.UsageCount = 1;
            
            await tagRepository.CreateTag(tag);

            return tag.Id;
        }
        catch (Exception e)
        {
            logger.Error(e,
                "{ClassName}::{MethodName} - ERROR while creating a new tag - Tag: {TagName}. Error: {ErrorMessage}",
                className, methodName, rawTag.Name, e.Message);
            throw;
        }
    }

    private async Task UpdateExistingTag(Guid tagId)
    {
        const string className = nameof(PostUpdatedEventConsumer);
        const string methodName = nameof(UpdateExistingTag);

        try
        {
            var existedTag = await tagRepository.GetTagById(tagId);
            if (existedTag == null)
            {
                logger.Warning(ErrorMessagesConsts.Tag.TagNotFound);
                return;
            }

            // Increase the usage count for the existing tag (Tăng usage count cho tag hiện tại)
            existedTag.UsageCount++;
            
            await tagRepository.UpdateTag(existedTag);
        }
        catch (Exception e)
        {
            logger.Error(e,
                "{ClassName}::{MethodName} - ERROR while updating an existing tag - TagId: {TagId}. Error: {ErrorMessage}",
                className, methodName, tagId, e.Message);
            throw;
        }
    }

    private async Task DecreaseTagUsageCount(Guid tagId)
    {
        const string className = nameof(PostUpdatedEventConsumer);
        const string methodName = nameof(DecreaseTagUsageCount);

        try
        {
            var tag = await tagRepository.GetTagById(tagId);
            if (tag == null)
            {
                logger.Warning("{ClassName}::{MethodName} - Tag not found - TagId: {TagId}", className, methodName,
                    tagId);
                return;
            }

            // Decrease the usage count for the removed tag (Giảm usage count cho tag bị xóa)
            tag.UsageCount--;
            
            await tagRepository.UpdateTag(tag);
        }
        catch (Exception e)
        {
            logger.Error(e,
                "{ClassName}::{MethodName} - ERROR while decreasing tag usage count - TagId: {TagId}. Error: {ErrorMessage}",
                className, methodName, tagId, e.Message);
            throw;
        }
    }

    #endregion
}