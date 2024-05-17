namespace Post.Domain.Services;

public interface IPostEmailTemplateService
{
    Task SendApprovedPostEmail(Guid postId, string name, string? content, string? description);
    
    Task SendPostSubmissionForApprovalEmail(Guid postId, string name);
    
    Task SendPostRejectionEmail(string name, string? reason);
}