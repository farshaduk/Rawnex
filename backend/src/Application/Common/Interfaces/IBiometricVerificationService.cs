namespace Rawnex.Application.Common.Interfaces;

public interface IBiometricVerificationService
{
    Task<BiometricResult> CreateVerificationSessionAsync(Guid userId, string redirectUrl, CancellationToken ct = default);
    Task<BiometricResult> GetVerificationResultAsync(string sessionId, CancellationToken ct = default);
}

public record BiometricResult(
    string SessionId,
    string Status,
    bool IsVerified,
    decimal? ConfidenceScore,
    string? ReviewUrl,
    string? RejectionReason);
