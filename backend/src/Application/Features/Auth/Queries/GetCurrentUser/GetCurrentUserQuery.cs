using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Auth.DTOs;

namespace Rawnex.Application.Features.Auth.Queries.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<Result<UserDto>>;
