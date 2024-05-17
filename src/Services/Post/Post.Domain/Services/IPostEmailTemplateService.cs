namespace Post.Domain.Services;

public interface IPostEmailTemplateService
{
    Task SendApprovedPostEmail(Guid postId, string name, string? content, string? description);
    
    Task SendPostSubmissionForApprovalEmail(Guid postId, string name, string? content, string? description);
    
    Task SendPostRejectionEmail(Guid postId, string name, string? reason, string? note);
}