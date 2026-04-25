using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Exceptions;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Companies.DTOs;
using Rawnex.Domain.Entities;

namespace Rawnex.Application.Features.Companies.Queries;

public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery, Result<CompanyDetailDto>>
{
    private readonly IApplicationDbContext _db;

    public GetCompanyByIdQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<CompanyDetailDto>> Handle(GetCompanyByIdQuery request, CancellationToken ct)
    {
        var company = await _db.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CompanyId, ct);

        if (company is null)
            throw new NotFoundException(nameof(Company), request.CompanyId);

        var members = await _db.CompanyMembers
            .AsNoTracking()
            .Where(m => m.CompanyId == request.CompanyId)
            .Include(m => m.User)
            .Include(m => m.Department)
            .Select(m => new CompanyMemberDto(
                m.Id, m.UserId, m.User.Email!, $"{m.User.FirstName} {m.User.LastName}",
                m.DepartmentId, m.Department != null ? m.Department.Name : null,
                m.JobTitle, m.IsCompanyAdmin, m.CreatedAt))
            .ToListAsync(ct);

        var documents = await _db.CompanyDocuments
            .AsNoTracking()
            .Where(d => d.CompanyId == request.CompanyId)
            .Select(d => new CompanyDocumentDto(
                d.Id, d.Type, d.FileName, d.FileUrl, d.VerificationStatus, d.ExpiresAt, d.CreatedAt))
            .ToListAsync(ct);

        var departments = await _db.Departments
            .AsNoTracking()
            .Where(d => d.CompanyId == request.CompanyId)
            .Select(d => new DepartmentDto(
                d.Id, d.Name, d.Type, d.ParentDepartmentId, null))
            .ToListAsync(ct);

        return Result<CompanyDetailDto>.Success(new CompanyDetailDto(
            company.Id, company.TenantId, company.LegalName, company.TradeName,
            company.RegistrationNumber, company.TaxId, company.Type, company.Status,
            company.VerificationStatus, company.AddressLine1, company.AddressLine2,
            company.City, company.State, company.Country, company.PostalCode,
            company.Phone, company.Email, company.Website, company.LogoUrl,
            company.BankName, company.BankAccountNumber, company.BankSwiftCode, company.BankIban,
            company.TrustScore, company.EsgScore, company.ParentCompanyId, company.CreatedAt,
            members, documents, departments));
    }
}

public class GetMyCompaniesQueryHandler : IRequestHandler<GetMyCompaniesQuery, Result<IList<CompanyDto>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetMyCompaniesQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<IList<CompanyDto>>> Handle(GetMyCompaniesQuery request, CancellationToken ct)
    {
        if (_currentUser.UserId is null)
            return Result<IList<CompanyDto>>.Failure("Not authenticated.");

        var companies = await _db.CompanyMembers
            .AsNoTracking()
            .Where(m => m.UserId == _currentUser.UserId.Value)
            .Select(m => m.Company)
            .Select(c => new CompanyDto(
                c.Id, c.TenantId, c.LegalName, c.TradeName, c.RegistrationNumber,
                c.Type, c.Status, c.VerificationStatus, c.City, c.Country, c.Website,
                c.TrustScore, c.EsgScore, c.CreatedAt))
            .ToListAsync(ct);

        return Result<IList<CompanyDto>>.Success(companies);
    }
}

public class SearchCompaniesQueryHandler : IRequestHandler<SearchCompaniesQuery, Result<PaginatedList<CompanyDto>>>
{
    private readonly IApplicationDbContext _db;

    public SearchCompaniesQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<PaginatedList<CompanyDto>>> Handle(SearchCompaniesQuery request, CancellationToken ct)
    {
        var query = _db.Companies.AsNoTracking()
            .Where(c => c.VerificationStatus == Domain.Enums.VerificationStatus.Approved);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(c =>
                c.LegalName.ToLower().Contains(term) ||
                (c.TradeName != null && c.TradeName.ToLower().Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(request.Country))
            query = query.Where(c => c.Country == request.Country);

        var result = await query
            .OrderBy(c => c.LegalName)
            .Select(c => new CompanyDto(
                c.Id, c.TenantId, c.LegalName, c.TradeName, c.RegistrationNumber,
                c.Type, c.Status, c.VerificationStatus, c.City, c.Country, c.Website,
                c.TrustScore, c.EsgScore, c.CreatedAt))
            .ToPaginatedListAsync(request.PageNumber, request.PageSize, ct);

        return Result<PaginatedList<CompanyDto>>.Success(result);
    }
}
