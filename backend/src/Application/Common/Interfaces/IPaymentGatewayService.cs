namespace Rawnex.Application.Common.Interfaces;

public interface IPaymentGatewayService
{
    Task<PaymentIntentResult> CreatePaymentIntentAsync(decimal amount, string currency, string? description = null, Dictionary<string, string>? metadata = null, CancellationToken ct = default);
    Task<PaymentIntentResult> ConfirmPaymentIntentAsync(string paymentIntentId, CancellationToken ct = default);
    Task<RefundResult> RefundPaymentAsync(string paymentIntentId, decimal? amount = null, CancellationToken ct = default);
    Task<string> CreateCustomerAsync(string email, string name, CancellationToken ct = default);
}

public record PaymentIntentResult(
    string PaymentIntentId,
    string ClientSecret,
    string Status,
    decimal Amount,
    string Currency);

public record RefundResult(
    string RefundId,
    string Status,
    decimal Amount);
