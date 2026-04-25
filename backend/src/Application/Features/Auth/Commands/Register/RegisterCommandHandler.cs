using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Auth.DTOs;
using Rawnex.Domain.Entities;
using Rawnex.Domain.Enums;
using Rawnex.Domain.Events;

namespace Rawnex.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<UserDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuditService _auditService;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        UserManager<ApplicationUser> userManager,
        IAuditService auditService,
        ILogger<RegisterCommandHandler> logger)
    {
        _userManager = userManager;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<Result<UserDto>> Handle(RegisterCommand request, CancellationToken ct)
    {
        // Check if email exists
        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
            return Result<UserDto>.Failure("A user with this email already exists.");

        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            EmailConfirmed = true, // Change if you want email confirmation flow
        };

        user.AddDomainEvent(new UserRegisteredEvent(user.Id, user.Email));

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return Result<UserDto>.Failure(errors.AsReadOnly());
        }

        // Default role
        await _userManager.AddToRoleAsync(user, ApplicationRole.User);
        var roles = await _userManager.GetRolesAsync(user);

        await _auditService.LogAsync(AuditAction.UserCreated, user.Id, user.Email,
            $"Registered with role: {ApplicationRole.User}", request.IpAddress, request.UserAgent, true, ct);

        _logger.LogInformation("User {Email} registered successfully", user.Email);

        return Result<UserDto>.Success(new UserDto(
            user.Id, user.Email!, user.FirstName, user.LastName, user.FullName, roles, [], user.IsActive));
    }
}
