using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly HttpClient _httpClient;

    public EmailService(IOptions<EmailSettings> settings, HttpClient httpClient)
    {
        _settings = settings.Value;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://api.resend.com/");
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _settings.ResendApiKey);
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string htmlBody)
    {
        var requestData = new
        {
            from = $"{_settings.SenderName} <{_settings.SenderEmail}>",
            to = new[] { to },
            subject,
            html = htmlBody
        };

        var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("emails", content);

        return response.IsSuccessStatusCode;
    }
}
