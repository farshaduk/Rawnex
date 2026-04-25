using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Negotiations.DTOs;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Negotiations.Commands;

public record StartNegotiationCommand(
    Guid BuyerCompanyId,
    Guid SellerCompanyId,
    Guid? RfqId,
    Guid? ListingId,
    string? Subject,
    string InitialMessage
) : IRequest<Result<NegotiationDto>>;

public record SendNegotiationMessageCommand(
    Guid NegotiationId,
    Guid SenderCompanyId,
    string Content,
    bool IsCounterOffer = false,
    string? CounterOfferJson = null
) : IRequest<Result<NegotiationMessageDto>>;

public record AcceptNegotiationCommand(
    Guid NegotiationId,
    decimal AgreedPrice,
    Currency AgreedCurrency,
    decimal? AgreedQuantity,
    Incoterm? AgreedIncoterm
) : IRequest<Result>;

public record RejectNegotiationCommand(Guid NegotiationId) : IRequest<Result>;
