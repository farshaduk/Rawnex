using Rawnex.Application.Common.Interfaces;
using Rawnex.Domain.Entities;
using Rawnex.Domain.Enums;

namespace Rawnex.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly IApplicationDbContext _dbContext;

    public AuditService(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task LogAsync(AuditAction action, Guid? userId, string? email, string? details,
        string? ipAddress, string? userAgent, bool isSuccess, CancellationToken ct = default)
    {
        var log = new AuditLog
        {
            UserId = userId,
            UserEmail = email,
            Action = action,
            Details = details,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            IsSuccess = isSuccess,
        };

        _dbContext.AuditLogs.Add(log);
        await _dbContext.SaveChangesAsync(ct);
    }
}
