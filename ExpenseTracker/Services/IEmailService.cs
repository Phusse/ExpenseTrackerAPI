namespace ExpenseTracker.Services;

/// <summary>
/// Defines the contract for sending emails within the application.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends a standard email asynchronously.
    /// </summary>
    /// <param name="to">The recipient's email address.</param>
    /// <param name="subject">The subject of the email.</param>
    /// <param name="htmlBody">The HTML body content of the email.</param>
    /// <param name="plainText">Optional plain-text fallback body. Used if the recipient's client does not support HTML.</param>
    /// <returns>Returns <c>true</c> if the email was sent successfully; otherwise, <c>false</c>.</returns>
    Task<bool> SendEmailAsync(string to, string subject, string htmlBody, string? plainText = null);

    /// <summary>
    /// Sends a template-based email asynchronously.
    /// </summary>
    /// <param name="to">The recipient's email address.</param>
    /// <param name="templateId">The ID of the email template to use.</param>
    /// <param name="templateModel">The object containing data to populate the template placeholders.</param>
    /// <returns>Returns <c>true</c> if the email was sent successfully; otherwise, <c>false</c>.</returns>
    Task<bool> SendTemplateEmailAsync(string to, int templateId, object templateModel);
}
