namespace Rawnex.Application.Common.Interfaces;

public interface ISanctionScreeningService
{
    Task<SanctionScreeningResult> ScreenCompanyAsync(string companyName, string country, CancellationToken ct = default);
    Task<SanctionScreeningResult> ScreenIndividualAsync(string fullName, string? dateOfBirth, string? country, CancellationToken ct = default);
}

public record SanctionScreeningResult(
    bool IsMatch,
    decimal ConfidenceScore,
    IReadOnlyList<SanctionHit> Hits,
    DateTime ScreenedAt);

public record SanctionHit(
    string ListName,
    string MatchedName,
    decimal Score,
    string? EntityType,
    string? Country);
