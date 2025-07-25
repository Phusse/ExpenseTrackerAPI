using PostmarkDotNet;

namespace ExpenseTracker.Services;

/// <summary>
/// Provides email-related services using configuration settings.
/// </summary>
internal class EmailService(IConfiguration config) : IEmailService
{
    private readonly string _postmarkToken = config["Postmark:Token"]
        ?? throw new InvalidOperationException($"Configuration setting 'Postmark:Token' is missing or invalid");

    private readonly string _fromEmail = config["Postmark:FromEmail"]
        ?? throw new InvalidOperationException($"Configuration setting 'Postmark:FromEmail' is missing or invalid");

    public async Task<bool> SendEmailAsync(string to, string subject, string htmlBody, string? plainText = null)
    {
        PostmarkClient client = new(_postmarkToken);

        PostmarkMessage message = new()
        {
            To = to,
            From = _fromEmail,
            Subject = subject,
            HtmlBody = htmlBody,
            TextBody = plainText ?? "This is a fallback plain-text version.",
            TrackOpens = true,
        };

        PostmarkResponse result = await client.SendMessageAsync(message);
        return result.Status == PostmarkStatus.Success;
    }

    public async Task<bool> SendTemplateEmailAsync(string to, int templateId, object templateModel)
    {
        PostmarkClient client = new(_postmarkToken);

        PostmarkResponse result = await client.SendEmailWithTemplateAsync(new TemplatedPostmarkMessage
        {
            To = to,
            From = _fromEmail,
            TemplateId = templateId,
            TemplateModel = templateModel
        });

        return result.Status == PostmarkStatus.Success;
    }
}
