using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Verification.DTOs;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Verification.Queries;

public record GetMyKycStatusQuery : IRequest<Result<KycVerificationDto?>>;

public record GetPendingKycListQuery(int PageNumber = 1, int PageSize = 20) : IRequest<Result<PaginatedList<KycVerificationDto>>>;

public record GetMyKybStatusQuery(Guid CompanyId) : IRequest<Result<KybVerificationDto?>>;

public record GetPendingKybListQuery(int PageNumber = 1, int PageSize = 20) : IRequest<Result<PaginatedList<KybVerificationDto>>>;
