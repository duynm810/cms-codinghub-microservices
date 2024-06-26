using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.Tag;
using Tag.Api.Services.Interfaces;

namespace Tag.Api.Controllers;

public static class TagsController
{
    public static void MapTagApi(this WebApplication app)
    {
        app.MapPost("/api/tags", async ([FromServices] ITagService tagService, [FromBody] CreateTagDto request) =>
        {
            var result = await tagService.CreateTag(request);
            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
        }).RequireAuthorization("Bearer");

        app.MapPut("/api/tags/{id:guid}", async ([FromServices] ITagService tagService, Guid id, UpdateTagDto request) =>
        {
            var result = await tagService.UpdateTag(id, request);
            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
        }).RequireAuthorization("Bearer");
        
        app.MapDelete("/api/tags", async ([FromServices] ITagService tagService, [FromBody] List<Guid> ids) =>
        {
            var result = await tagService.DeleteTag(ids);
            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
        }).RequireAuthorization("Bearer");

        app.MapGet("/api/tags", async ([FromServices] ITagService tagService, [FromQuery] int count = 4) =>
        {
            var result = await tagService.GetTags(count);
            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
        });

        app.MapGet("/api/tags/{id:guid}", async ([FromServices] ITagService tagService, Guid id) =>
        {
            var result = await tagService.GetTagById(id);
            return result.IsSuccess ? Results.Ok(result) : Results.NotFound(result);
        }).RequireAuthorization("Bearer");
        
        app.MapGet("/api/tags/by-slug/{slug}", async ([FromServices] ITagService tagService, string slug) =>
        {
            var result = await tagService.GetTagBySlug(slug);
            return result.IsSuccess ? Results.Ok(result) : Results.NotFound(result);
        });
        
        app.MapGet("/api/tags/suggest", async ([FromServices] ITagService tagService, [FromQuery] int count = 5) =>
        {
            var result = await tagService.GetSuggestedTags(count);
            return result.IsSuccess ? Results.Ok(result) : Results.NotFound(result);
        });
    }
}