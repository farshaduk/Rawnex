namespace Rawnex.Application.Features.Auth.DTOs;

public record SessionDto(
    Guid SessionId,
    string DeviceInfo,
    string? IpAddress,
    DateTime CreatedAt,
    DateTime LastActivityAt,
    bool IsCurrent
);
