using Rawnex.Domain.Enums;

namespace Rawnex.Application.Common.Interfaces;

public interface IFraudScoringService
{
    Task<FraudAssessment> AssessUserAsync(Guid userId, string? ipAddress, string? deviceFingerprint, CancellationToken ct = default);
    Task<FraudAssessment> AssessCompanyAsync(Guid companyId, CancellationToken ct = default);
    Task<FraudAssessment> AssessTransactionAsync(Guid orderId, decimal amount, CancellationToken ct = default);
}

public record FraudAssessment(decimal Score, RiskLevel RiskLevel, bool IsFlagged, string? FlagReason, Dictionary<string, object>? Details);
