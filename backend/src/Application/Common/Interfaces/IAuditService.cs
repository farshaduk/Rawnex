using Rawnex.Domain.Entities;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Common.Interfaces;

public interface IAuditService
{
    Task LogAsync(AuditAction action, Guid? userId, string? email, string? details,
        string? ipAddress, string? userAgent, bool isSuccess, CancellationToken ct = default);
}
