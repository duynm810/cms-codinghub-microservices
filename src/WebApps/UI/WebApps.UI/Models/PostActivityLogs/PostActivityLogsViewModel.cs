using Shared.Dtos.PostActivity;

namespace WebApps.UI.Models.PostActivityLogs;

public class PostActivityLogsViewModel
{
    public List<PostActivityLogDto> PostActivityLogs { get; set; } = default!;
}