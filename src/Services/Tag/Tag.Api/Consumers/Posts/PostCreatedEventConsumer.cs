using AutoMapper;
using EventBus.IntegrationEvents;
using EventBus.IntegrationEvents.Interfaces;
using MassTransit;
using Microsoft.Extensions.Options;
using Shared.Constants;
using Shared.Dtos.Tag;
using Shared.Settings;
using Tag.Api.Entities;
using Tag.Api.Repositories.Interfaces;
using ILogger = Serilog.ILogger;

namespace Tag.Api.Consumers.Posts;

public class PostCreatedEventConsumer(
    IPublishEndpoint publishEndpoint,
    ITagRepository tagRepository,
    IOptions<EventBusSettings> eventBusSettings,
    IMapper mapper,
    ILogger logger) : IConsumer<IPostCreatedEvent>
{
    private readonly EventBusSettings _eventBusSettings = eventBusSettings.Value;

    public async Task Consume(ConsumeContext<IPostCreatedEvent> context)
    {
        var message = context.Message;

        const string methodName = nameof(Consume);
        const string className = nameof(PostCreatedEventConsumer);

        logger.Information("BEGIN processing {ClassName} - PostId: {PostId}", className, message.PostId);

        await using var transaction = await tagRepository.BeginTransactionAsync();

        try
        {
            var tagIds = new List<Guid>();

            var newTags = message.RawTags.Where(t => !t.IsExisting).ToList();
            var existingTags = message.RawTags.Where(t => t.IsExisting).ToList();

            foreach (var tag in newTags)
            {
                var tagId = await CreateNewTag(tag);
                tagIds.Add(tagId);
            }

            foreach (var tag in existingTags)
            {
                var tagId = await UpdateExistingTag(tag);
                if (tagId != Guid.Empty)
                {
                    tagIds.Add(tagId);
                }
            }

            await transaction.CommitAsync();

            // Publish event for created tags (Phát hành sự kiện cho các tags đã tạo)
            await PublishPostTagsCreatedEvent(message.PostId, tagIds);

            logger.Information("END processing {ClassName} successfully - PostId: {PostId}", className,
                message.PostId);
        }
        catch (Exception e)
        {
            await tagRepository.RollbackTransactionAsync();
            logger.Error(e,
                "ERROR while processing {MethodName} - PostId: {PostId}. Error: {ErrorMessage}", methodName,
                message.PostId, e.Message);
            throw;
        }
    }

    #region HELPERS

    private async Task PublishPostTagsCreatedEvent(Guid postId, List<Guid> tagIds)
    {
        const string methodName = nameof(PublishPostTagsCreatedEvent);

        var serviceName = _eventBusSettings.ServiceName;

        logger.Information("BEGIN Publish {MethodName} - PostId: {PostId}, SourceService: {SourceService}", methodName,
            postId, serviceName);

        try
        {
            var postTagsCreatedEvent = new PostTagsCreatedEvent(_eventBusSettings.ServiceName)
            {
                PostId = postId,
                TagIds = tagIds
            };

            await publishEndpoint.Publish<IPostTagsCreatedEvent>(postTagsCreatedEvent);
            
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
        const string className = nameof(PostCreatedEventConsumer);
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

    private async Task<Guid> UpdateExistingTag(RawTagDto rawTag)
    {
        const string className = nameof(PostCreatedEventConsumer);
        const string methodName = nameof(UpdateExistingTag);

        try
        {
            var existedTag = await tagRepository.GetTagById(Guid.Parse(rawTag.Id));
            if (existedTag == null)
            {
                logger.Warning(ErrorMessagesConsts.Tag.TagNotFound);
                return Guid.Empty;
            }

            // Increase the usage count for the existing tag (Tăng usage count cho tag hiện tại)
            existedTag.UsageCount++;
            
            await tagRepository.UpdateTag(existedTag);

            return existedTag.Id;
        }
        catch (Exception e)
        {
            logger.Error(e,
                "{ClassName}::{MethodName} - ERROR while updating an existing tag - TagId: {TagId}. Error: {ErrorMessage}",
                className, methodName, rawTag.Id, e.Message);
            throw;
        }
    }

    #endregion
}