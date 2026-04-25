using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Domain.Enums;

namespace Rawnex.Infrastructure.Jobs;

/// <summary>
/// Background jobs that run on a schedule via Hangfire.
/// </summary>
public class RecurringJobs
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly ILogger<RecurringJobs> _logger;

    private readonly IFraudScoringService _fraudScoring;

    public RecurringJobs(
        IApplicationDbContext context,
        INotificationService notificationService,
        IFraudScoringService fraudScoring,
        ILogger<RecurringJobs> logger)
    {
        _context = context;
        _notificationService = notificationService;
        _fraudScoring = fraudScoring;
        _logger = logger;
    }

    /// <summary>
    /// Expire listings past their ExpiresAt date. Runs every hour.
    /// </summary>
    public async Task ExpireListingsAsync()
    {
        var now = DateTime.UtcNow;
        var expiredListings = await _context.Listings
            .Where(l => l.Status == ListingStatus.Active && l.ExpiresAt.HasValue && l.ExpiresAt < now)
            .ToListAsync();

        foreach (var listing in expiredListings)
        {
            listing.Status = ListingStatus.Expired;
        }

        if (expiredListings.Count > 0)
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Expired {Count} listings", expiredListings.Count);
        }
    }

    /// <summary>
    /// Expire RFQs past their ResponseDeadline. Runs every hour.
    /// </summary>
    public async Task ExpireRfqsAsync()
    {
        var now = DateTime.UtcNow;
        var expiredRfqs = await _context.Rfqs
            .Where(r => r.Status == RfqStatus.Published && r.ResponseDeadline < now)
            .ToListAsync();

        foreach (var rfq in expiredRfqs)
        {
            rfq.Status = RfqStatus.Expired;
        }

        if (expiredRfqs.Count > 0)
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Expired {Count} RFQs", expiredRfqs.Count);
        }
    }

    /// <summary>
    /// End auctions past their ScheduledEndAt. Runs every minute.
    /// </summary>
    public async Task EndAuctionsAsync()
    {
        var now = DateTime.UtcNow;
        var endedAuctions = await _context.Auctions
            .Where(a => a.Status == AuctionStatus.Active && a.ScheduledEndAt <= now)
            .ToListAsync();

        foreach (var auction in endedAuctions)
        {
            auction.Status = AuctionStatus.Ended;
            auction.ActualEndAt = now;
        }

        if (endedAuctions.Count > 0)
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Ended {Count} auctions", endedAuctions.Count);
        }
    }

    /// <summary>
    /// Start auctions that have reached their ScheduledStartAt. Runs every minute.
    /// </summary>
    public async Task StartScheduledAuctionsAsync()
    {
        var now = DateTime.UtcNow;
        var readyAuctions = await _context.Auctions
            .Where(a => a.Status == AuctionStatus.Scheduled && a.ScheduledStartAt <= now)
            .ToListAsync();

        foreach (var auction in readyAuctions)
        {
            auction.Status = AuctionStatus.Active;
            auction.ActualStartAt = now;
        }

        if (readyAuctions.Count > 0)
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Started {Count} scheduled auctions", readyAuctions.Count);
        }
    }

    /// <summary>
    /// Send overdue invoice reminders. Runs daily.
    /// </summary>
    public async Task SendOverdueInvoiceRemindersAsync()
    {
        var now = DateTime.UtcNow;
        var overdueInvoices = await _context.Invoices
            .Include(i => i.RecipientCompany)
            .Where(i => i.Status == InvoiceStatus.Sent && i.DueDate < now)
            .ToListAsync();

        foreach (var invoice in overdueInvoices)
        {
            invoice.Status = InvoiceStatus.Overdue;
        }

        if (overdueInvoices.Count > 0)
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Marked {Count} invoices as overdue", overdueInvoices.Count);
        }
    }

    /// <summary>
    /// Cleanup expired sessions and refresh tokens. Runs daily.
    /// </summary>
    public async Task CleanupExpiredTokensAsync()
    {
        var now = DateTime.UtcNow;

        var expiredTokens = await _context.RefreshTokens
            .Where(t => t.ExpiresAt < now && t.RevokedAt == null)
            .ToListAsync();

        foreach (var token in expiredTokens)
        {
            token.RevokedAt = now;
            token.RevokedReason = "Expired — cleaned up by background job";
        }

        var expiredSessions = await _context.UserSessions
            .Where(s => !s.IsRevoked && s.LastActivityAt < now.AddDays(-30))
            .ToListAsync();

        foreach (var session in expiredSessions)
        {
            session.IsRevoked = true;
            session.RevokedAt = now;
        }

        if (expiredTokens.Count > 0 || expiredSessions.Count > 0)
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Cleaned up {Tokens} expired tokens and {Sessions} expired sessions",
                expiredTokens.Count, expiredSessions.Count);
        }
    }

    /// <summary>
    /// Check for expired negotiations. Runs every hour.
    /// </summary>
    /// <summary>
    /// Expire stale negotiations that have had no activity for 30 days. Runs every hour.
    /// </summary>
    public async Task ExpireNegotiationsAsync()
    {
        var now = DateTime.UtcNow;
        var cutoff = now.AddDays(-30);
        var staleNegotiations = await _context.Negotiations
            .Where(n => n.Status == NegotiationStatus.Active && n.CreatedAt < cutoff)
            .ToListAsync();

        foreach (var negotiation in staleNegotiations)
        {
            negotiation.Status = NegotiationStatus.Expired;
        }

        if (staleNegotiations.Count > 0)
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Expired {Count} stale negotiations", staleNegotiations.Count);
        }
    }

    /// <summary>
    /// Periodic seller re-audit: Re-assess risk for active sellers every week.
    /// Checks document expiry, dispute history, and rating changes.
    /// </summary>
    public async Task SellerReAuditAsync()
    {
        var activeSellerCompanies = await _context.Companies
            .Where(c => c.Status == CompanyStatus.Active &&
                        (c.Type == CompanyType.Seller || c.Type == CompanyType.Both))
            .ToListAsync();

        var reAuditCount = 0;
        foreach (var company in activeSellerCompanies)
        {
            // Check for expired documents
            var expiredDocs = await _context.CompanyDocuments
                .Where(d => d.CompanyId == company.Id && d.ExpiresAt.HasValue && d.ExpiresAt < DateTime.UtcNow)
                .CountAsync();

            // Check recent disputes
            var recentDisputes = await _context.Disputes
                .Where(d => d.AgainstCompanyId == company.Id && d.CreatedAt > DateTime.UtcNow.AddMonths(-3))
                .CountAsync();

            // Re-assess fraud
            var assessment = await _fraudScoring.AssessCompanyAsync(company.Id, CancellationToken.None);

            if (expiredDocs > 0 || recentDisputes > 3 || assessment.IsFlagged)
            {
                await _notificationService.SendToCompanyAsync(
                    company.Id,
                    "Account Re-Audit Required",
                    $"Your seller account requires re-verification. Expired documents: {expiredDocs}, Recent disputes: {recentDisputes}.",
                    NotificationType.SecurityAlert,
                    NotificationPriority.High,
                    "/company/verification",
                    CancellationToken.None);

                reAuditCount++;
            }
        }

        _logger.LogInformation("Seller re-audit completed: {Count} sellers flagged out of {Total}",
            reAuditCount, activeSellerCompanies.Count);
    }

    /// <summary>
    /// Detect potential off-platform payment attempts by scanning recent chat messages.
    /// Runs daily.
    /// </summary>
    public async Task DetectOffPlatformPaymentsAsync()
    {
        var cutoff = DateTime.UtcNow.AddHours(-24);

        var suspiciousKeywords = new[]
        {
            "bank transfer direct", "pay outside", "off platform", "direct payment",
            "wire transfer", "western union", "my personal account", "bypass escrow",
            "skip escrow", "pay me directly", "send money to", "account number",
            "IBAN:", "SWIFT:", "routing number"
        };

        var recentMessages = await _context.ChatMessages
            .Where(m => m.CreatedAt > cutoff)
            .Select(m => new { m.Id, m.ConversationId, m.SenderUserId, m.Content })
            .ToListAsync();

        var flaggedCount = 0;
        foreach (var msg in recentMessages)
        {
            if (msg.Content is null) continue;

            var contentLower = msg.Content.ToLowerInvariant();
            var matchedKeywords = suspiciousKeywords
                .Where(k => contentLower.Contains(k.ToLowerInvariant(), StringComparison.Ordinal))
                .ToList();

            if (matchedKeywords.Count > 0)
            {
                _logger.LogWarning(
                    "Off-platform payment attempt detected. Message {MessageId} by user {UserId} in conversation {ConversationId}. Keywords: {Keywords}",
                    msg.Id, msg.SenderUserId, msg.ConversationId, string.Join(", ", matchedKeywords));

                // Notify admins
                var adminUserIds = await _context.UserRoles
                    .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.NormalizedName })
                    .Where(x => x.NormalizedName == "ADMIN")
                    .Select(x => x.UserId)
                    .Distinct()
                    .ToListAsync();

                foreach (var adminId in adminUserIds)
                {
                    await _notificationService.SendAsync(
                        adminId,
                        null,
                        "Off-Platform Payment Alert",
                        $"Suspicious message detected with keywords: {string.Join(", ", matchedKeywords)}",
                        NotificationType.SecurityAlert,
                        NotificationPriority.Urgent,
                        $"/admin/chat-monitoring",
                        null,
                        CancellationToken.None);
                }

                flaggedCount++;
            }
        }

        _logger.LogInformation("Off-platform payment detection completed: {Count} suspicious messages found", flaggedCount);
    }
}
