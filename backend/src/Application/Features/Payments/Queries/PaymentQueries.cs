using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Payments.DTOs;

namespace Rawnex.Application.Features.Payments.Queries;

public record GetEscrowByOrderQuery(Guid PurchaseOrderId) : IRequest<Result<EscrowAccountDto>>;
public record GetPaymentsByOrderQuery(Guid PurchaseOrderId) : IRequest<Result<List<PaymentDto>>>;
public record GetInvoiceByIdQuery(Guid InvoiceId) : IRequest<Result<InvoiceDetailDto>>;
public record GetCompanyInvoicesQuery(Guid CompanyId, int PageNumber = 1, int PageSize = 20) : IRequest<Result<PaginatedList<InvoiceDto>>>;
