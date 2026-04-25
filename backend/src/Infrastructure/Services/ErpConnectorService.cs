using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;

namespace Rawnex.Infrastructure.Services;

public class ErpConnectorService : IErpConnectorService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IApplicationDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ErpConnectorService> _logger;

    public ErpConnectorService(
        IHttpClientFactory httpClientFactory,
        IApplicationDbContext db,
        IConfiguration configuration,
        ILogger<ErpConnectorService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _db = db;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ErpSyncResult> SyncOrderAsync(Guid purchaseOrderId, CancellationToken ct = default)
    {
        var baseUrl = _configuration["ExternalApis:Erp:BaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            _logger.LogWarning("ERP not configured. Order sync for {OrderId} skipped", purchaseOrderId);
            return new ErpSyncResult(false, null, "ERP not configured", DateTime.UtcNow);
        }

        var order = await _db.PurchaseOrders.FindAsync(new object[] { purchaseOrderId }, ct);
        if (order is null)
            return new ErpSyncResult(false, null, "Order not found", DateTime.UtcNow);

        try
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync($"{baseUrl}/orders/sync", new
            {
                order.Id,
                order.OrderNumber,
                order.BuyerCompanyId,
                order.SellerCompanyId,
                order.TotalAmount,
                order.Status
            }, ct);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<JsonElement>(ct);
                var externalId = result.TryGetProperty("externalId", out var eid) ? eid.GetString() : null;
                _logger.LogInformation("Order {OrderId} synced to ERP: {ExternalId}", purchaseOrderId, externalId);
                return new ErpSyncResult(true, externalId, null, DateTime.UtcNow);
            }

            var error = await response.Content.ReadAsStringAsync(ct);
            return new ErpSyncResult(false, null, error, DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERP sync failed for order {OrderId}", purchaseOrderId);
            return new ErpSyncResult(false, null, ex.Message, DateTime.UtcNow);
        }
    }

    public async Task<ErpSyncResult> SyncInvoiceAsync(Guid invoiceId, CancellationToken ct = default)
    {
        var baseUrl = _configuration["ExternalApis:Erp:BaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl))
            return new ErpSyncResult(false, null, "ERP not configured", DateTime.UtcNow);

        var invoice = await _db.Invoices.FindAsync(new object[] { invoiceId }, ct);
        if (invoice is null)
            return new ErpSyncResult(false, null, "Invoice not found", DateTime.UtcNow);

        try
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync($"{baseUrl}/invoices/sync", new
            {
                invoice.Id,
                invoice.InvoiceNumber,
                invoice.TotalAmount,
                invoice.Status
            }, ct);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<JsonElement>(ct);
                var externalId = result.TryGetProperty("externalId", out var eid) ? eid.GetString() : null;
                return new ErpSyncResult(true, externalId, null, DateTime.UtcNow);
            }

            var error = await response.Content.ReadAsStringAsync(ct);
            return new ErpSyncResult(false, null, error, DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERP sync failed for invoice {InvoiceId}", invoiceId);
            return new ErpSyncResult(false, null, ex.Message, DateTime.UtcNow);
        }
    }

    public async Task<ErpSyncResult> SyncInventoryAsync(Guid warehouseId, CancellationToken ct = default)
    {
        var baseUrl = _configuration["ExternalApis:Erp:BaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl))
            return new ErpSyncResult(false, null, "ERP not configured", DateTime.UtcNow);

        try
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync($"{baseUrl}/inventory/sync", new { warehouseId }, ct);
            if (response.IsSuccessStatusCode)
                return new ErpSyncResult(true, null, null, DateTime.UtcNow);

            var error = await response.Content.ReadAsStringAsync(ct);
            return new ErpSyncResult(false, null, error, DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERP inventory sync failed for warehouse {WarehouseId}", warehouseId);
            return new ErpSyncResult(false, null, ex.Message, DateTime.UtcNow);
        }
    }

    public async Task<ErpSyncResult> PushPaymentAsync(Guid paymentId, CancellationToken ct = default)
    {
        var baseUrl = _configuration["ExternalApis:Erp:BaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl))
            return new ErpSyncResult(false, null, "ERP not configured", DateTime.UtcNow);

        var payment = await _db.Payments.FindAsync(new object[] { paymentId }, ct);
        if (payment is null)
            return new ErpSyncResult(false, null, "Payment not found", DateTime.UtcNow);

        try
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync($"{baseUrl}/payments/sync", new
            {
                payment.Id,
                payment.PaymentReference,
                payment.Amount,
                payment.Currency,
                payment.Status
            }, ct);

            if (response.IsSuccessStatusCode)
                return new ErpSyncResult(true, null, null, DateTime.UtcNow);

            var error = await response.Content.ReadAsStringAsync(ct);
            return new ErpSyncResult(false, null, error, DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERP payment sync failed for {PaymentId}", paymentId);
            return new ErpSyncResult(false, null, ex.Message, DateTime.UtcNow);
        }
    }

    private HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient("Erp");
        var apiKey = _configuration["ExternalApis:Erp:ApiKey"];
        if (!string.IsNullOrWhiteSpace(apiKey))
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-Api-Key", apiKey);
        return client;
    }
}
