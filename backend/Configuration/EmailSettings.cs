namespace ExpenseTracker.Configuration;

/// <summary>
/// Configuration settings for the email service.
/// </summary>
public class EmailSettings
{
    /// <summary>
    /// API key used to authenticate with the Resend email service.
    /// </summary>
    public string ResendApiKey { get; set; } = string.Empty;

    /// <summary>
    /// The email address used as the sender in outbound emails.
    /// </summary>
    public string SenderEmail { get; set; } = string.Empty;

    /// <summary>
    /// The name to display as the sender in outbound emails.
    /// </summary>
    public string SenderName { get; set; } = string.Empty;
}
