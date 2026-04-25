namespace Rawnex.Application.Features.Auth.DTOs;

public record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    IList<string> Roles,
    IList<string> Permissions,
    bool IsActive
);
