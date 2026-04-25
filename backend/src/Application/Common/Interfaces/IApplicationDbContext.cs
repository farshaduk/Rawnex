using Rawnex.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Rawnex.Application.Common.Interfaces;

/// <summary>
/// Used DIRECTLY by Query handlers for read operations (Hybrid CQRS read side).
/// No repository needed for reads — query DbContext directly.
/// </summary>
public interface IApplicationDbContext
{
    // Identity & Auth
    DbSet<ApplicationUser> Users { get; }
    DbSet<ApplicationRole> Roles { get; }
    DbSet<ApplicationUserRole> UserRoles { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<UserSession> UserSessions { get; }
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<UserPermission> UserPermissions { get; }

    // Tenancy
    DbSet<Tenant> Tenants { get; }
    DbSet<TenantSettings> TenantSettings { get; }

    // Organization
    DbSet<Company> Companies { get; }
    DbSet<Department> Departments { get; }
    DbSet<CompanyDocument> CompanyDocuments { get; }
    DbSet<UboRecord> UboRecords { get; }
    DbSet<CompanyMember> CompanyMembers { get; }
    DbSet<TrustedPartner> TrustedPartners { get; }

    // Catalog
    DbSet<ProductCategory> ProductCategories { get; }
    DbSet<Product> Products { get; }
    DbSet<ProductAttribute> ProductAttributes { get; }
    DbSet<ProductVariant> ProductVariants { get; }
    DbSet<ProductCertification> ProductCertifications { get; }
    DbSet<SavedSearch> SavedSearches { get; }
    DbSet<Wishlist> Wishlists { get; }
    DbSet<WishlistItem> WishlistItems { get; }
    DbSet<PriceAlert> PriceAlerts { get; }

    // Trading
    DbSet<Listing> Listings { get; }
    DbSet<Rfq> Rfqs { get; }
    DbSet<RfqResponse> RfqResponses { get; }
    DbSet<RfqInvitation> RfqInvitations { get; }
    DbSet<Auction> Auctions { get; }
    DbSet<AuctionBid> AuctionBids { get; }
    DbSet<Negotiation> Negotiations { get; }
    DbSet<NegotiationMessage> NegotiationMessages { get; }
    DbSet<PurchaseOrder> PurchaseOrders { get; }
    DbSet<PurchaseOrderItem> PurchaseOrderItems { get; }
    DbSet<OrderApproval> OrderApprovals { get; }

    // Contracts
    DbSet<Contract> Contracts { get; }
    DbSet<ContractClause> ContractClauses { get; }
    DbSet<DigitalSignature> DigitalSignatures { get; }

    // Payments
    DbSet<EscrowAccount> EscrowAccounts { get; }
    DbSet<EscrowMilestone> EscrowMilestones { get; }
    DbSet<Payment> Payments { get; }
    DbSet<Invoice> Invoices { get; }
    DbSet<InvoiceItem> InvoiceItems { get; }

    // Logistics
    DbSet<Shipment> Shipments { get; }
    DbSet<ShipmentTracking> ShipmentTrackings { get; }
    DbSet<FreightQuote> FreightQuotes { get; }
    DbSet<Warehouse> Warehouses { get; }
    DbSet<InventoryItem> InventoryItems { get; }
    DbSet<Batch> Batches { get; }

    // Quality
    DbSet<QualityInspection> QualityInspections { get; }
    DbSet<QualityReport> QualityReports { get; }
    DbSet<LabTestResult> LabTestResults { get; }

    // Dispute & Rating
    DbSet<Dispute> Disputes { get; }
    DbSet<DisputeEvidence> DisputeEvidences { get; }
    DbSet<Rating> Ratings { get; }

    // Notifications
    DbSet<Notification> Notifications { get; }
    DbSet<NotificationPreference> NotificationPreferences { get; }

    // Fraud & Compliance
    DbSet<FraudScore> FraudScores { get; }
    DbSet<SanctionCheck> SanctionChecks { get; }
    DbSet<KycVerification> KycVerifications { get; }
    DbSet<KybVerification> KybVerifications { get; }

    // Platform
    DbSet<CommissionRule> CommissionRules { get; }
    DbSet<PlatformBilling> PlatformBillings { get; }
    DbSet<FeatureFlag> FeatureFlags { get; }
    DbSet<EsgScore> EsgScores { get; }

    // Chat
    DbSet<ChatConversation> ChatConversations { get; }
    DbSet<ChatParticipant> ChatParticipants { get; }
    DbSet<ChatMessage> ChatMessages { get; }

    // Webhooks
    DbSet<WebhookSubscription> WebhookSubscriptions { get; }
    DbSet<WebhookDelivery> WebhookDeliveries { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
