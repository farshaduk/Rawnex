using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;

namespace Rawnex.Infrastructure.Services;

public class StripePaymentService : IPaymentGatewayService
{
    private const string BaseUrl = "https://api.stripe.com/v1";
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _secretKey;
    private readonly ILogger<StripePaymentService> _logger;

    public StripePaymentService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<StripePaymentService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _secretKey = configuration["Stripe:SecretKey"] ?? "";
        _logger = logger;
    }

    public async Task<PaymentIntentResult> CreatePaymentIntentAsync(
        decimal amount, string currency, string? description = null,
        Dictionary<string, string>? metadata = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_secretKey))
        {
            _logger.LogWarning("Stripe not configured. Returning mock payment intent.");
            var mockId = $"pi_mock_{Guid.NewGuid():N}";
            return new PaymentIntentResult(mockId, $"secret_{mockId}", "requires_payment_method", amount, currency);
        }

        var client = CreateClient();
        var amountInCents = (long)(amount * 100);

        var formData = new Dictionary<string, string>
        {
            ["amount"] = amountInCents.ToString(),
            ["currency"] = currency.ToLowerInvariant(),
            ["automatic_payment_methods[enabled]"] = "true"
        };

        if (description is not null)
            formData["description"] = description;

        if (metadata is not null)
        {
            foreach (var kvp in metadata)
                formData[$"metadata[{kvp.Key}]"] = kvp.Value;
        }

        var response = await client.PostAsync($"{BaseUrl}/payment_intents", new FormUrlEncodedContent(formData), ct);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>(ct);

        if (!response.IsSuccessStatusCode)
        {
            var errorMsg = json.TryGetProperty("error", out var err)
                ? err.GetProperty("message").GetString()
                : "Unknown Stripe error";
            _logger.LogError("Stripe CreatePaymentIntent failed: {Error}", errorMsg);
            throw new InvalidOperationException($"Stripe error: {errorMsg}");
        }

        return new PaymentIntentResult(
            json.GetProperty("id").GetString()!,
            json.GetProperty("client_secret").GetString()!,
            json.GetProperty("status").GetString()!,
            amount,
            currency);
    }

    public async Task<PaymentIntentResult> ConfirmPaymentIntentAsync(string paymentIntentId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_secretKey))
        {
            _logger.LogWarning("Stripe not configured. Returning mock confirmation.");
            return new PaymentIntentResult(paymentIntentId, "", "succeeded", 0, "usd");
        }

        var client = CreateClient();
        var response = await client.PostAsync($"{BaseUrl}/payment_intents/{paymentIntentId}/confirm",
            new FormUrlEncodedContent(new Dictionary<string, string>()), ct);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>(ct);

        if (!response.IsSuccessStatusCode)
        {
            var errorMsg = json.TryGetProperty("error", out var err)
                ? err.GetProperty("message").GetString()
                : "Unknown Stripe error";
            throw new InvalidOperationException($"Stripe error: {errorMsg}");
        }

        return new PaymentIntentResult(
            json.GetProperty("id").GetString()!,
            json.GetProperty("client_secret").GetString()!,
            json.GetProperty("status").GetString()!,
            json.GetProperty("amount").GetInt64() / 100m,
            json.GetProperty("currency").GetString()!);
    }

    public async Task<RefundResult> RefundPaymentAsync(string paymentIntentId, decimal? amount = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_secretKey))
        {
            _logger.LogWarning("Stripe not configured. Returning mock refund.");
            return new RefundResult($"re_mock_{Guid.NewGuid():N}", "succeeded", amount ?? 0);
        }

        var client = CreateClient();
        var formData = new Dictionary<string, string> { ["payment_intent"] = paymentIntentId };
        if (amount.HasValue)
            formData["amount"] = ((long)(amount.Value * 100)).ToString();

        var response = await client.PostAsync($"{BaseUrl}/refunds", new FormUrlEncodedContent(formData), ct);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>(ct);

        if (!response.IsSuccessStatusCode)
        {
            var errorMsg = json.TryGetProperty("error", out var err)
                ? err.GetProperty("message").GetString()
                : "Unknown Stripe error";
            throw new InvalidOperationException($"Stripe error: {errorMsg}");
        }

        return new RefundResult(
            json.GetProperty("id").GetString()!,
            json.GetProperty("status").GetString()!,
            json.GetProperty("amount").GetInt64() / 100m);
    }

    public async Task<string> CreateCustomerAsync(string email, string name, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_secretKey))
        {
            _logger.LogWarning("Stripe not configured. Returning mock customer ID.");
            return $"cus_mock_{Guid.NewGuid():N}";
        }

        var client = CreateClient();
        var formData = new Dictionary<string, string>
        {
            ["email"] = email,
            ["name"] = name
        };

        var response = await client.PostAsync($"{BaseUrl}/customers", new FormUrlEncodedContent(formData), ct);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>(ct);

        if (!response.IsSuccessStatusCode)
        {
            var errorMsg = json.TryGetProperty("error", out var err)
                ? err.GetProperty("message").GetString()
                : "Unknown Stripe error";
            throw new InvalidOperationException($"Stripe error: {errorMsg}");
        }

        return json.GetProperty("id").GetString()!;
    }

    private HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient("Stripe");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _secretKey);
        return client;
    }
}
