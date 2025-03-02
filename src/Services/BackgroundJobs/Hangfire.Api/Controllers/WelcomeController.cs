using Contracts.Scheduled;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace Hangfire.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WelcomeController(IScheduledJobService jobService, ILogger logger) : ControllerBase
{
    [HttpPost]
    [Route("[action]")]
    public IActionResult Welcome()
    {
        var jobId = jobService.Enqueue(() => ResponseWelcome("Welcome to Hangfire API"));
        return Ok($"Job ID: {jobId} - Enqueue Job");
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult DelayedWelcome()
    {
        var seconds = 5;
        var jobId = jobService.Schedule(() => ResponseWelcome("Welcome to Hangfire API"),
            TimeSpan.FromSeconds(seconds));
        return Ok($"Job ID: {jobId} - Delayed Job");
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult WelcomeAt()
    {
        var enqueueAt = DateTimeOffset.UtcNow.AddSeconds(10);
        var jobId = jobService.Schedule(() => ResponseWelcome("Welcome to Hangfire API"),
            enqueueAt);
        return Ok($"Job ID: {jobId} - Schedule Job");
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult ConfirmedWelcome()
    {
        const int timeInSeconds = 5;
        var parentJobId =
            jobService.Schedule(() => ResponseWelcome("Welcome to Hangfire API"), TimeSpan.FromSeconds(5));

        var jobId = jobService.ContinueQueueWith(parentJobId,
            () => ResponseWelcome("Welcome message is sent"));

        return Ok($"Job ID: {jobId} - Confirmed Welcome will be sent in {timeInSeconds} seconds");
    }

    [NonAction]
    public void ResponseWelcome(string text)
    {
        logger.Information(text);
    }
}