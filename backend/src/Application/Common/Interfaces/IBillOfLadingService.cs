namespace Rawnex.Application.Common.Interfaces;

public interface IBillOfLadingService
{
    Task<BillOfLadingResult> GenerateAsync(Guid shipmentId, CancellationToken ct = default);
}

public record BillOfLadingResult(
    bool IsSuccess,
    string? BolNumber,
    string? FileUrl,
    string? ErrorMessage);
