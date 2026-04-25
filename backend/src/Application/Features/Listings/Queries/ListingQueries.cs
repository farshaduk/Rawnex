using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Listings.DTOs;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Listings.Queries;

public record GetListingByIdQuery(Guid ListingId) : IRequest<Result<ListingDetailDto>>;

public record SearchListingsQuery(
    ListingType? Type,
    Guid? CompanyId,
    Guid? ProductId,
    string? SearchTerm,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<Result<PaginatedList<ListingDto>>>;
