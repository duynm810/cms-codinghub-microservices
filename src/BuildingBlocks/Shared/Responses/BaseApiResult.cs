namespace Shared.Responses;

public class BaseApiResult
{
    // Indicates if the request was successful.
    public bool IsSuccess { get; set; }

    // HTTP status code for the response.
    public int StatusCode { get; set; }

    // List of messages, which could be errors or other information.
    public List<string> Messages { get; set; } = [];

    // Optional: Title or summary of the response.
    public string? Title { get; set; }

    // Time taken to process the request (in milliseconds).
    public long Duration { get; set; }

    // Timestamp when the response was generated.
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}