using EasyApply.BusinessLayer.Interfaces.Services;
using FluentEmail.Core;
using Microsoft.Extensions.Logging;

namespace EasyApply.BusinessLayer.Core;

public class EmailService : IEmailService
{
    private readonly IFluentEmailFactory _emailFactory;
    private readonly ILogger<EmailService> _logger;
    private readonly string _templatePath;

    public EmailService(IFluentEmailFactory emailFactory, ILogger<EmailService> logger)
    {
        _emailFactory = emailFactory;
        _logger = logger;
        // Templates are copied to the output directory
        _templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "Emails");
    }

    public async Task SendWelcomeEmailAsync(string email, string name)
    {
        try
        {
            var templateFile = Path.Combine(_templatePath, "Welcome.cshtml");
            var result = await _emailFactory
                .Create()
                .To(email)
                .Subject("Welcome to EasyApply!")
                .UsingTemplateFromFile(templateFile, new { Name = name })
                .SendAsync();

            if (!result.Successful)
            {
                _logger.LogError("Failed to send welcome email to {Email}: {Errors}", email, string.Join(", ", result.ErrorMessages));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while sending welcome email to {Email}", email);
        }
    }

    public async Task SendApplicationReceivedEmailAsync(string email, string name, string jobTitle)
    {
        try
        {
            var templateFile = Path.Combine(_templatePath, "ApplicationReceived.cshtml");
            var result = await _emailFactory
                .Create()
                .To(email)
                .Subject($"Application Received: {jobTitle}")
                .UsingTemplateFromFile(templateFile, new { Name = name, JobTitle = jobTitle })
                .SendAsync();

            if (!result.Successful)
            {
                _logger.LogError("Failed to send application confirmation to {Email}: {Errors}", email, string.Join(", ", result.ErrorMessages));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while sending application confirmation to {Email}", email);
        }
    }

    public async Task SendInterviewInviteEmailAsync(string email, string name, string jobTitle, DateTime scheduledAt)
    {
        try
        {
            var templateFile = Path.Combine(_templatePath, "InterviewInvite.cshtml");
            var result = await _emailFactory
                .Create()
                .To(email)
                .Subject($"Interview Invitation: {jobTitle}")
                .UsingTemplateFromFile(templateFile, new { Name = name, JobTitle = jobTitle, ScheduledAt = scheduledAt })
                .SendAsync();

            if (!result.Successful)
            {
                _logger.LogError("Failed to send interview invite to {Email}: {Errors}", email, string.Join(", ", result.ErrorMessages));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while sending interview invite to {Email}", email);
        }
    }
}
