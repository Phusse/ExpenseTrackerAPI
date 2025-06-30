public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string htmlBody, string plainText = null);
    Task<bool> SendTemplateEmailAsync(string to, int templateId, object templateModel);
}
