namespace EasyApply.BusinessLayer.Interfaces.Services;

public interface IEmailService
{
    Task SendWelcomeEmailAsync(string email, string name);
    Task SendApplicationReceivedEmailAsync(string email, string name, string jobTitle);
    Task SendInterviewInviteEmailAsync(string email, string name, string jobTitle, DateTime scheduledAt);
    Task SendApplicationRejectionEmailAsync(string email, string name, string jobTitle, string feedback);
}
