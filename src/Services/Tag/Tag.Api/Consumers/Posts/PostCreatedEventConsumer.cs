using AutoMapper;
using EventBus.IntegrationEvents;
using EventBus.IntegrationEvents.Interfaces;
using MassTransit;
using Shared.Constants;
using Shared.Dtos.Tag;
using Tag.Api.Entities;
using Tag.Api.Repositories.Interfaces;
using ILogger = Serilog.ILogger;

namespace Tag.Api.Consumers.Posts;

public class PostCreatedEventConsumer(IPublishEndpoint publishEndpoint, ITagRepository tagRepository, IMapper mapper, ILogger logger) : IConsumer<IPostCreatedEvent>
{
    public async Task Consume(ConsumeContext<IPostCreatedEvent> context)
    {
        var postCreatedEvent = context.Message;
        const string methodName = nameof(Consume);

        logger.Information("Handling PostCreatedEventConsumer - PostId: {PostId}", postCreatedEvent.PostId);

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
        }
        catch (Exception e)
        {
            await tagRepository.RollbackTransactionAsync();
            logger.Error(e, "{MethodName} - An error occurred while handling PostCreatedEvent - PostId: {PostId}", methodName, postCreatedEvent.PostId);
            throw;
        }
    }

    #region Helpers
    
    private async Task PublishTagsProcessedEvent(Guid postId, List<Guid> tagIds)
    {
        const string methodName = nameof(PublishTagsProcessedEvent);

        try
        {
            var tagProcessedEvent = new TagProcessedEvent()
            {
                PostId = postId,
                TagIds = tagIds
            };
            
            await publishEndpoint.Publish<ITagProcessedEvent>(tagProcessedEvent);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "{MethodName} - An error occurred while publishing TagProcessedEvent - PostId: {PostId}. Error: {ErrorMessage}", methodName, postId, ex.Message);
            throw;
        }
    }

    private async Task<Guid> CreateNewTag(RawTagDto rawTag)
    {
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
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred while creating a new tag - Tag: {TagName}. Error: {ErrorMessage}", rawTag.Name, ex.Message);
            throw;
        }
    }

    private async Task<Guid> UpdateExistingTag(RawTagDto rawTag)
    {
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
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred while updating an existing tag - TagId: {TagId}. Error: {ErrorMessage}", rawTag.Id, ex.Message);
            throw;
        }
    }

    #endregion
}