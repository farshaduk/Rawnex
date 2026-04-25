using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Domain.Entities;
using Rawnex.Domain.Enums;

namespace Rawnex.Infrastructure.Services;

public class FraudScoringService : IFraudScoringService
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<FraudScoringService> _logger;

    public FraudScoringService(IApplicationDbContext context, ILogger<FraudScoringService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<FraudAssessment> AssessUserAsync(Guid userId, string? ipAddress, string? deviceFingerprint, CancellationToken ct)
    {
        var details = new Dictionary<string, object>();
        decimal score = 0;

        // Check recent login patterns
        var recentSessions = await _context.UserSessions
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .Take(20)
            .ToListAsync(ct);

        var uniqueIps = recentSessions.Select(s => s.IpAddress).Distinct().Count();
        if (uniqueIps > 10)
        {
            score += 20;
            details["high_ip_diversity"] = uniqueIps;
        }

        // Check if user has previous fraud flags
        var existingFlags = await _context.FraudScores
            .Where(f => f.UserId == userId && f.IsFlagged)
            .CountAsync(ct);

        if (existingFlags > 0)
        {
            score += 30;
            details["previous_flags"] = existingFlags;
        }

        // Check account age
        var user = await _context.Users.FindAsync(new object[] { userId }, ct);
        if (user != null)
        {
            var accountAge = (DateTime.UtcNow - user.CreatedAt).TotalDays;
            if (accountAge < 7)
            {
                score += 15;
                details["new_account_days"] = accountAge;
            }
        }

        var riskLevel = CalculateRiskLevel(score);
        var isFlagged = score >= 60;
        string? flagReason = isFlagged ? "Automated fraud detection: high risk score" : null;

        var fraudScore = new FraudScore
        {
            UserId = userId,
            CheckType = FraudCheckType.BehavioralPattern,
            RiskLevel = riskLevel,
            Score = score,
            DetailsJson = JsonSerializer.Serialize(details),
            IpAddress = ipAddress,
            DeviceFingerprint = deviceFingerprint,
            IsFlagged = isFlagged,
            FlagReason = flagReason
        };

        _context.FraudScores.Add(fraudScore);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Fraud assessment for user {UserId}: Score={Score}, Risk={Risk}", userId, score, riskLevel);

        return new FraudAssessment(score, riskLevel, isFlagged, flagReason, details);
    }

    public async Task<FraudAssessment> AssessCompanyAsync(Guid companyId, CancellationToken ct)
    {
        var details = new Dictionary<string, object>();
        decimal score = 0;

        var company = await _context.Companies.FindAsync(new object[] { companyId }, ct);
        if (company == null)
            return new FraudAssessment(0, RiskLevel.Low, false, null, null);

        // Check sanction screening
        var sanctionMatches = await _context.SanctionChecks
            .Where(s => s.CompanyId == companyId && s.IsMatch)
            .CountAsync(ct);

        if (sanctionMatches > 0)
        {
            score += 50;
            details["sanction_matches"] = sanctionMatches;
        }

        // Check document verification
        var docs = await _context.CompanyDocuments
            .Where(d => d.CompanyId == companyId)
            .ToListAsync(ct);

        if (docs.Count == 0)
        {
            score += 20;
            details["no_documents"] = true;
        }

        // Check rating
        var avgRating = await _context.Ratings
            .Where(r => r.ReviewedCompanyId == companyId)
            .AverageAsync(r => (decimal?)r.OverallScore, ct) ?? 0;

        if (avgRating < 2 && avgRating > 0)
        {
            score += 15;
            details["low_rating"] = avgRating;
        }

        // Check dispute history
        var disputeCount = await _context.Disputes
            .Where(d => d.AgainstCompanyId == companyId)
            .CountAsync(ct);

        if (disputeCount > 5)
        {
            score += 20;
            details["high_disputes"] = disputeCount;
        }

        var riskLevel = CalculateRiskLevel(score);
        var isFlagged = score >= 60;

        var fraudScore = new FraudScore
        {
            CompanyId = companyId,
            CheckType = FraudCheckType.SanctionScreening,
            RiskLevel = riskLevel,
            Score = score,
            DetailsJson = JsonSerializer.Serialize(details),
            IsFlagged = isFlagged,
            FlagReason = isFlagged ? "Automated company risk assessment: high risk" : null
        };

        _context.FraudScores.Add(fraudScore);
        await _context.SaveChangesAsync(ct);

        return new FraudAssessment(score, riskLevel, isFlagged, fraudScore.FlagReason, details);
    }

    public async Task<FraudAssessment> AssessTransactionAsync(Guid orderId, decimal amount, CancellationToken ct)
    {
        var details = new Dictionary<string, object>();
        decimal score = 0;

        var order = await _context.PurchaseOrders
            .Include(o => o.BuyerCompany)
            .FirstOrDefaultAsync(o => o.Id == orderId, ct);

        if (order == null)
            return new FraudAssessment(0, RiskLevel.Low, false, null, null);

        // Large transaction check
        if (amount > 1_000_000)
        {
            score += 15;
            details["large_amount"] = amount;
        }

        // Check buyer's transaction history
        var buyerOrderCount = await _context.PurchaseOrders
            .Where(o => o.BuyerCompanyId == order.BuyerCompanyId)
            .CountAsync(ct);

        if (buyerOrderCount <= 2 && amount > 100_000)
        {
            score += 25;
            details["new_buyer_large_order"] = true;
        }

        // Check velocity — multiple orders in short period
        var recentOrders = await _context.PurchaseOrders
            .Where(o => o.BuyerCompanyId == order.BuyerCompanyId && o.CreatedAt > DateTime.UtcNow.AddHours(-24))
            .CountAsync(ct);

        if (recentOrders > 10)
        {
            score += 20;
            details["high_velocity"] = recentOrders;
        }

        var riskLevel = CalculateRiskLevel(score);
        var isFlagged = score >= 60;

        var fraudScore = new FraudScore
        {
            CompanyId = order.BuyerCompanyId,
            CheckType = FraudCheckType.Transaction,
            RiskLevel = riskLevel,
            Score = score,
            DetailsJson = JsonSerializer.Serialize(details),
            IsFlagged = isFlagged,
            FlagReason = isFlagged ? $"High risk transaction: Order {orderId}" : null
        };

        _context.FraudScores.Add(fraudScore);
        await _context.SaveChangesAsync(ct);

        return new FraudAssessment(score, riskLevel, isFlagged, fraudScore.FlagReason, details);
    }

    private static RiskLevel CalculateRiskLevel(decimal score) => score switch
    {
        < 15 => RiskLevel.VeryLow,
        < 30 => RiskLevel.Low,
        < 50 => RiskLevel.Medium,
        < 70 => RiskLevel.High,
        _ => RiskLevel.VeryHigh
    };
}
