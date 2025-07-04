using PostmarkDotNet;

public class EmailService : IEmailService
{
    private readonly string _postmarkToken;
    private readonly string _fromEmail;

    public EmailService(IConfiguration config)
    {
        _postmarkToken = config["Postmark:Token"];
        _fromEmail = config["Postmark:FromEmail"];
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string htmlBody, string? plainText = null)
    {
        var client = new PostmarkClient(_postmarkToken);

        var message = new PostmarkMessage
        {
            To = to,
            From = _fromEmail,
            Subject = subject,
            HtmlBody = htmlBody,
            TextBody = plainText ?? "This is a fallback plain-text version.",
            TrackOpens = true
        };

        var result = await client.SendMessageAsync(message);
        return result.Status == PostmarkStatus.Success;
    }

    public async Task<bool> SendTemplateEmailAsync(string to, int templateId, object templateModel)
    {
        var client = new PostmarkClient(_postmarkToken);

        var result = await client.SendEmailWithTemplateAsync(new TemplatedPostmarkMessage
        {
            To = to,
            From = _fromEmail,
            TemplateId = templateId,
            TemplateModel = templateModel
        });

        return result.Status == PostmarkStatus.Success;
    }
}
