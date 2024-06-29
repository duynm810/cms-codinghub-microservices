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

public class PostCreatedEventConsumer(IPublishEndpoint publishEndpoint, ITagRepository tagRepository, IOptions<EventBusSettings> eventBusSettings, IMapper mapper, ILogger logger) : IConsumer<IPostCreatedEvent>
{
    private readonly EventBusSettings _eventBusSettings = eventBusSettings.Value;
    
    public async Task Consume(ConsumeContext<IPostCreatedEvent> context)
    {
        var postCreatedEvent = context.Message;
        
        const string methodName = nameof(Consume);
        const string className = nameof(PostCreatedEventConsumer);

        logger.Information("BEGIN processing {ClassName} - PostId: {PostId}", className, postCreatedEvent.PostId);

        await using var transaction = await tagRepository.BeginTransactionAsync();
            
        try
        {
            var tagIds = new List<Guid>();
                
            foreach (var rawTag in postCreatedEvent.RawTags)
            {
                if (!rawTag.IsExisting)
                {
                    var newTagId = await CreateNewTag(rawTag);
                    tagIds.Add(newTagId);
                }
                else
                {
                    var existingTagId = await UpdateExistingTag(rawTag);
                    if (existingTagId != Guid.Empty)
                    {
                        tagIds.Add(existingTagId);
                    }
                }
            }

            await transaction.CommitAsync();
                
            await PublishTagsProcessedEvent(postCreatedEvent.PostId, tagIds);
            
            logger.Information("END processing {ClassName} successfully - PostId: {PostId}", className, postCreatedEvent.PostId);
        }
        catch (Exception e)
        {
            await tagRepository.RollbackTransactionAsync();
            logger.Error(e, "{ClassName}::{MethodName} - ERROR while processing PostCreatedEvent - PostId: {PostId}. Error: {ErrorMessage}", className, methodName, postCreatedEvent.PostId, e.Message);
            throw;
        }
    }

    #region Helpers
    
    private async Task PublishTagsProcessedEvent(Guid postId, List<Guid> tagIds)
    {
        const string methodName = nameof(PublishTagsProcessedEvent);
        const string className = nameof(PostCreatedEventConsumer);
        
        try
        {
            var tagProcessedEvent = new TagProcessedEvent(_eventBusSettings.ServiceName)
            {
                PostId = postId,
                TagIds = tagIds
            };
            
            await publishEndpoint.Publish<ITagProcessedEvent>(tagProcessedEvent);
        }
        catch (Exception e)
        {
            logger.Error(e, "{ClassName}::{MethodName} - ERROR while publishing TagProcessedEvent - PostId: {PostId}. Error: {ErrorMessage}", className, methodName, postId, e.Message);
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
            
            tag.UsageCount = 1;
            await tagRepository.CreateTag(tag);
            
            return tag.Id;
        }
        catch (Exception e)
        {
            logger.Error(e, "{ClassName}::{MethodName} - ERROR while creating a new tag - Tag: {TagName}. Error: {ErrorMessage}", className, methodName, rawTag.Name, e.Message);
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

            existedTag.UsageCount++;
            await tagRepository.UpdateTag(existedTag);
            
            return existedTag.Id;
        }
        catch (Exception e)
        {
            logger.Error(e, "{ClassName}::{MethodName} - ERROR while updating an existing tag - TagId: {TagId}. Error: {ErrorMessage}", className, methodName, rawTag.Id, e.Message);
            throw;
        }
    }

    #endregion
}