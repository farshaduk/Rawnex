namespace Rawnex.Application.Common.Interfaces;

public interface IErpConnectorService
{
    Task<ErpSyncResult> SyncOrderAsync(Guid purchaseOrderId, CancellationToken ct = default);
    Task<ErpSyncResult> SyncInvoiceAsync(Guid invoiceId, CancellationToken ct = default);
    Task<ErpSyncResult> SyncInventoryAsync(Guid warehouseId, CancellationToken ct = default);
    Task<ErpSyncResult> PushPaymentAsync(Guid paymentId, CancellationToken ct = default);
}

public record ErpSyncResult(bool IsSuccess, string? ExternalId, string? ErrorMessage, DateTime SyncedAt);
