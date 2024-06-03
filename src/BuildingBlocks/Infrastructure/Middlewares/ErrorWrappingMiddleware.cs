using System.Text.Json;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Serilog;
using Shared.Responses;

namespace Infrastructure.Middlewares;

public class ErrorWrappingMiddleware(RequestDelegate next, ILogger logger)
{
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task Invoke(HttpContext context)
    {
        var response = new ApiResult<bool>();

        var errorMsg = string.Empty;
        try
        {
            await next.Invoke(context);
        }
        catch (ValidationException e)
        {
            _logger.Error(e, e.Message);
            errorMsg = e.Errors.FirstOrDefault().Value.FirstOrDefault();
            context.Response.StatusCode = StatusCodes.Status406NotAcceptable;
        }
        catch (Exception e)
        {
            _logger.Error(e, e.Message);
            errorMsg = e.Message;
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }

        if ((!context.Response.HasStarted && context.Response.StatusCode == StatusCodes.Status401Unauthorized) ||
            context.Response.StatusCode == StatusCodes.Status403Forbidden)
        {
            context.Response.ContentType = "application/json";

            response.Error("Unauthorized");

            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }

        else if (!context.Response.HasStarted && context.Response.StatusCode != StatusCodes.Status204NoContent &&
                 context.Response.StatusCode != StatusCodes.Status202Accepted &&
                 context.Response.StatusCode != StatusCodes.Status200OK &&
                 context.Response.ContentType != "text/html; charset=utf-8")
        {
            context.Response.ContentType = "application/json";

            if (!string.IsNullOrEmpty(errorMsg))
            {
                response.Error(errorMsg);
            }

            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
    }
}