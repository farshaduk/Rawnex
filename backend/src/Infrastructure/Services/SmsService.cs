using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;

namespace Rawnex.Infrastructure.Services;

public class SmsService : ISmsService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmsService> _logger;

    public SmsService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<SmsService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendAsync(string phoneNumber, string message, CancellationToken ct = default)
    {
        var provider = _configuration["Sms:Provider"];
        var apiKey = _configuration["Sms:ApiKey"];

        if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning("SMS not configured. Message to {Phone} skipped", phoneNumber);
            return;
        }

        try
        {
            var client = _httpClientFactory.CreateClient("Sms");

            if (provider.Equals("kavenegar", StringComparison.OrdinalIgnoreCase))
            {
                var url = $"https://api.kavenegar.com/v1/{apiKey}/sms/send.json";
                var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["receptor"] = phoneNumber,
                    ["message"] = message,
                    ["sender"] = _configuration["Sms:Sender"] ?? "10008663"
                });
                var response = await client.PostAsync(url, content, ct);
                response.EnsureSuccessStatusCode();
            }
            else if (provider.Equals("twilio", StringComparison.OrdinalIgnoreCase))
            {
                var accountSid = _configuration["Sms:AccountSid"]!;
                var url = $"https://api.twilio.com/2010-04-01/Accounts/{accountSid}/Messages.json";
                var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["To"] = phoneNumber,
                    ["From"] = _configuration["Sms:Sender"] ?? "",
                    ["Body"] = message
                });
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{accountSid}:{apiKey}")));
                var response = await client.PostAsync(url, content, ct);
                response.EnsureSuccessStatusCode();
            }

            _logger.LogInformation("SMS sent to {Phone}", phoneNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send SMS to {Phone}", phoneNumber);
            throw;
        }
    }
}
