using AutoMapper;
using EventBus.IntegrationEvents.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;
using Shared.Dtos.Tag;
using Tag.Api.Entities;
using Tag.Api.Repositories.Interfaces;
using ILogger = Serilog.ILogger;

namespace Tag.Api.Consumers.Posts;

public class PostCreatedEventConsumer(ITagRepository tagRepository, IMapper mapper, ILogger logger) : IConsumer<IPostCreatedEvent>
{
    public async Task Consume(ConsumeContext<IPostCreatedEvent> context)
    {
        var postCreatedEvent = context.Message;
        const string methodName = nameof(Consume);

        logger.Information("Handling PostCreatedEventConsumer - PostId: {PostId}", postCreatedEvent.PostId);

        var strategy = tagRepository.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await tagRepository.BeginTransactionAsync();
            
            try
            {
                foreach (var rawTag in postCreatedEvent.RawTags)
                {
                    if (!rawTag.IsExisting)
                    {
                        await CreateNewTag(rawTag);
                    }
                    else
                    {
                        await UpdateExistingTag(rawTag);
                    }
                }

                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await tagRepository.RollbackTransactionAsync();
                logger.Error(e, "{MethodName} - An error occurred while handling PostCreatedEvent - PostId: {PostId}", methodName, postCreatedEvent.PostId);
                throw;
            }
        });

    }
    
    private async Task CreateNewTag(RawTagDto rawTag)
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
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred while creating a new tag - Tag: {TagName}", rawTag.Name);
            throw;
        }
    }

    private async Task UpdateExistingTag(RawTagDto rawTag)
    {
        try
        {
            var existedTag = await tagRepository.GetTagById(Guid.Parse(rawTag.Id));
            if (existedTag == null)
            {
                logger.Warning(ErrorMessagesConsts.Tag.TagNotFound);
                return;
            }

            existedTag.UsageCount++;
            await tagRepository.UpdateTag(existedTag);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred while updating an existing tag - TagId: {TagId}", rawTag.Id);
            throw;
        }
    }
}