using Microsoft.AspNetCore.Identity;
using Rawnex.Domain.Common;

namespace Rawnex.Domain.Entities;

/// <summary>
/// Application user extending ASP.NET Core Identity.
/// This is the aggregate root for user-related operations.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>, IAggregateRoot, ITenantEntity
{
    public Guid TenantId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public bool IsActive { get; set; } = true;

    // Audit
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public string? LastModifiedBy { get; set; }

    // Soft delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation
    public Tenant? Tenant { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<UserSession> Sessions { get; set; } = new List<UserSession>();
    public ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
    public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    public ICollection<CompanyMember> CompanyMemberships { get; set; } = new List<CompanyMember>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    // Domain events
    private readonly List<BaseDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<BaseDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public void AddDomainEvent(BaseDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();

    public string FullName => $"{FirstName} {LastName}";
}
