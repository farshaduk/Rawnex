using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Context;

public class ApplicationDbContext
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid,
        Microsoft.AspNetCore.Identity.IdentityUserClaim<Guid>,
        ApplicationUserRole,
        Microsoft.AspNetCore.Identity.IdentityUserLogin<Guid>,
        Microsoft.AspNetCore.Identity.IdentityRoleClaim<Guid>,
        Microsoft.AspNetCore.Identity.IdentityUserToken<Guid>>,
      IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // Auth entities
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    // Permission entities
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserPermission> UserPermissions => Set<UserPermission>();

    // Expose UserRoles for query handlers
    public new DbSet<ApplicationUserRole> UserRoles => Set<ApplicationUserRole>();

    // Tenancy
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TenantSettings> TenantSettings => Set<TenantSettings>();

    // Organization
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<CompanyDocument> CompanyDocuments => Set<CompanyDocument>();
    public DbSet<UboRecord> UboRecords => Set<UboRecord>();
    public DbSet<CompanyMember> CompanyMembers => Set<CompanyMember>();
    public DbSet<TrustedPartner> TrustedPartners => Set<TrustedPartner>();

    // Catalog
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductAttribute> ProductAttributes => Set<ProductAttribute>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<ProductCertification> ProductCertifications => Set<ProductCertification>();
    public DbSet<SavedSearch> SavedSearches => Set<SavedSearch>();
    public DbSet<Wishlist> Wishlists => Set<Wishlist>();
    public DbSet<WishlistItem> WishlistItems => Set<WishlistItem>();
    public DbSet<PriceAlert> PriceAlerts => Set<PriceAlert>();

    // Trading
    public DbSet<Listing> Listings => Set<Listing>();
    public DbSet<Rfq> Rfqs => Set<Rfq>();
    public DbSet<RfqResponse> RfqResponses => Set<RfqResponse>();
    public DbSet<RfqInvitation> RfqInvitations => Set<RfqInvitation>();
    public DbSet<Auction> Auctions => Set<Auction>();
    public DbSet<AuctionBid> AuctionBids => Set<AuctionBid>();
    public DbSet<Negotiation> Negotiations => Set<Negotiation>();
    public DbSet<NegotiationMessage> NegotiationMessages => Set<NegotiationMessage>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();
    public DbSet<OrderApproval> OrderApprovals => Set<OrderApproval>();

    // Contracts
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<ContractClause> ContractClauses => Set<ContractClause>();
    public DbSet<DigitalSignature> DigitalSignatures => Set<DigitalSignature>();

    // Payments
    public DbSet<EscrowAccount> EscrowAccounts => Set<EscrowAccount>();
    public DbSet<EscrowMilestone> EscrowMilestones => Set<EscrowMilestone>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();

    // Logistics
    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<ShipmentTracking> ShipmentTrackings => Set<ShipmentTracking>();
    public DbSet<FreightQuote> FreightQuotes => Set<FreightQuote>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<Batch> Batches => Set<Batch>();

    // Quality
    public DbSet<QualityInspection> QualityInspections => Set<QualityInspection>();
    public DbSet<QualityReport> QualityReports => Set<QualityReport>();
    public DbSet<LabTestResult> LabTestResults => Set<LabTestResult>();

    // Dispute & Rating
    public DbSet<Dispute> Disputes => Set<Dispute>();
    public DbSet<DisputeEvidence> DisputeEvidences => Set<DisputeEvidence>();
    public DbSet<Rating> Ratings => Set<Rating>();

    // Notifications
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();

    // Fraud & Compliance
    public DbSet<FraudScore> FraudScores => Set<FraudScore>();
    public DbSet<SanctionCheck> SanctionChecks => Set<SanctionCheck>();
    public DbSet<KycVerification> KycVerifications => Set<KycVerification>();
    public DbSet<KybVerification> KybVerifications => Set<KybVerification>();

    // Platform
    public DbSet<CommissionRule> CommissionRules => Set<CommissionRule>();
    public DbSet<PlatformBilling> PlatformBillings => Set<PlatformBilling>();
    public DbSet<FeatureFlag> FeatureFlags => Set<FeatureFlag>();
    public DbSet<EsgScore> EsgScores => Set<EsgScore>();

    // Chat
    public DbSet<ChatConversation> ChatConversations => Set<ChatConversation>();
    public DbSet<ChatParticipant> ChatParticipants => Set<ChatParticipant>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

    // Webhooks
    public DbSet<WebhookSubscription> WebhookSubscriptions => Set<WebhookSubscription>();
    public DbSet<WebhookDelivery> WebhookDeliveries => Set<WebhookDelivery>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // BaseDomainEvent is not a persisted entity — ignore it
        builder.Ignore<Domain.Common.BaseDomainEvent>();

        // Apply all IEntityTypeConfiguration from this assembly
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Prevent SQL Server cascade cycle errors by defaulting all FKs to Restrict
        foreach (var relationship in builder.Model.GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }

        // Rename Identity tables to clean names
        builder.Entity<ApplicationUser>(b => b.ToTable("Users"));
        builder.Entity<ApplicationRole>(b => b.ToTable("Roles"));
        builder.Entity<ApplicationUserRole>(b => b.ToTable("UserRoles"));
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<Guid>>(b => b.ToTable("UserClaims"));
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<Guid>>(b => b.ToTable("UserLogins"));
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<Guid>>(b => b.ToTable("RoleClaims"));
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<Guid>>(b => b.ToTable("UserTokens"));

        // Permission tables
        builder.Entity<Permission>(b => b.ToTable("Permissions"));
        builder.Entity<RolePermission>(b => b.ToTable("RolePermissions"));
        builder.Entity<UserPermission>(b => b.ToTable("UserPermissions"));

        // Tenancy tables
        builder.Entity<Tenant>(b => b.ToTable("Tenants"));
        builder.Entity<TenantSettings>(b => b.ToTable("TenantSettings"));

        // Organization tables
        builder.Entity<Company>(b => b.ToTable("Companies"));
        builder.Entity<Department>(b => b.ToTable("Departments"));
        builder.Entity<CompanyDocument>(b => b.ToTable("CompanyDocuments"));
        builder.Entity<UboRecord>(b => b.ToTable("UboRecords"));
        builder.Entity<CompanyMember>(b => b.ToTable("CompanyMembers"));
        builder.Entity<TrustedPartner>(b => b.ToTable("TrustedPartners"));

        // Catalog tables
        builder.Entity<ProductCategory>(b => b.ToTable("ProductCategories"));
        builder.Entity<Product>(b => b.ToTable("Products"));
        builder.Entity<ProductAttribute>(b => b.ToTable("ProductAttributes"));
        builder.Entity<ProductVariant>(b => b.ToTable("ProductVariants"));
        builder.Entity<ProductCertification>(b => b.ToTable("ProductCertifications"));
        builder.Entity<SavedSearch>(b => b.ToTable("SavedSearches"));
        builder.Entity<Wishlist>(b => b.ToTable("Wishlists"));
        builder.Entity<WishlistItem>(b => b.ToTable("WishlistItems"));
        builder.Entity<PriceAlert>(b => b.ToTable("PriceAlerts"));

        // Trading tables
        builder.Entity<Listing>(b => b.ToTable("Listings"));
        builder.Entity<Rfq>(b => b.ToTable("Rfqs"));
        builder.Entity<RfqResponse>(b => b.ToTable("RfqResponses"));
        builder.Entity<RfqInvitation>(b => b.ToTable("RfqInvitations"));
        builder.Entity<Auction>(b => b.ToTable("Auctions"));
        builder.Entity<AuctionBid>(b => b.ToTable("AuctionBids"));
        builder.Entity<Negotiation>(b => b.ToTable("Negotiations"));
        builder.Entity<NegotiationMessage>(b => b.ToTable("NegotiationMessages"));
        builder.Entity<PurchaseOrder>(b => b.ToTable("PurchaseOrders"));
        builder.Entity<PurchaseOrderItem>(b => b.ToTable("PurchaseOrderItems"));
        builder.Entity<OrderApproval>(b => b.ToTable("OrderApprovals"));

        // Contract tables
        builder.Entity<Contract>(b => b.ToTable("Contracts"));
        builder.Entity<ContractClause>(b => b.ToTable("ContractClauses"));
        builder.Entity<DigitalSignature>(b => b.ToTable("DigitalSignatures"));

        // Payment tables
        builder.Entity<EscrowAccount>(b => b.ToTable("EscrowAccounts"));
        builder.Entity<EscrowMilestone>(b => b.ToTable("EscrowMilestones"));
        builder.Entity<Payment>(b => b.ToTable("Payments"));
        builder.Entity<Invoice>(b => b.ToTable("Invoices"));
        builder.Entity<InvoiceItem>(b => b.ToTable("InvoiceItems"));

        // Logistics tables
        builder.Entity<Shipment>(b => b.ToTable("Shipments"));
        builder.Entity<ShipmentTracking>(b => b.ToTable("ShipmentTrackings"));
        builder.Entity<FreightQuote>(b => b.ToTable("FreightQuotes"));
        builder.Entity<Warehouse>(b => b.ToTable("Warehouses"));
        builder.Entity<InventoryItem>(b => b.ToTable("InventoryItems"));
        builder.Entity<Batch>(b => b.ToTable("Batches"));

        // Quality tables
        builder.Entity<QualityInspection>(b => b.ToTable("QualityInspections"));
        builder.Entity<QualityReport>(b => b.ToTable("QualityReports"));
        builder.Entity<LabTestResult>(b => b.ToTable("LabTestResults"));

        // Dispute & Rating tables
        builder.Entity<Dispute>(b => b.ToTable("Disputes"));
        builder.Entity<DisputeEvidence>(b => b.ToTable("DisputeEvidences"));
        builder.Entity<Rating>(b => b.ToTable("Ratings"));

        // Notification tables
        builder.Entity<Notification>(b => b.ToTable("Notifications"));
        builder.Entity<NotificationPreference>(b => b.ToTable("NotificationPreferences"));

        // Fraud & Compliance tables
        builder.Entity<FraudScore>(b => b.ToTable("FraudScores"));
        builder.Entity<SanctionCheck>(b => b.ToTable("SanctionChecks"));

        // Platform tables
        builder.Entity<CommissionRule>(b => b.ToTable("CommissionRules"));
        builder.Entity<PlatformBilling>(b => b.ToTable("PlatformBillings"));
        builder.Entity<FeatureFlag>(b => b.ToTable("FeatureFlags"));
        builder.Entity<EsgScore>(b => b.ToTable("EsgScores"));
    }
}
