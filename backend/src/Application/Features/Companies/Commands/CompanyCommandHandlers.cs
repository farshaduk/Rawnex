using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Exceptions;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Companies.DTOs;
using Rawnex.Domain.Entities;
using Rawnex.Domain.Enums;
using Rawnex.Domain.Events;

namespace Rawnex.Application.Features.Companies.Commands;

public class RegisterCompanyCommandHandler : IRequestHandler<RegisterCompanyCommand, Result<CompanyDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public RegisterCompanyCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<CompanyDto>> Handle(RegisterCompanyCommand request, CancellationToken ct)
    {
        if (_currentUser.UserId is null)
            return Result<CompanyDto>.Failure("Not authenticated.");

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == _currentUser.UserId.Value, ct);
        if (user is null)
            return Result<CompanyDto>.Failure("User not found.");

        var company = new Company
        {
            TenantId = user.TenantId,
            LegalName = request.LegalName,
            TradeName = request.TradeName,
            RegistrationNumber = request.RegistrationNumber,
            TaxId = request.TaxId,
            Type = request.Type,
            Status = CompanyStatus.Active,
            VerificationStatus = VerificationStatus.Pending,
            AddressLine1 = request.AddressLine1,
            City = request.City,
            State = request.State,
            Country = request.Country,
            PostalCode = request.PostalCode,
            Phone = request.Phone,
            Email = request.Email,
            Website = request.Website,
        };

        _db.Companies.Add(company);

        // Add the current user as company admin
        var member = new CompanyMember
        {
            CompanyId = company.Id,
            UserId = _currentUser.UserId.Value,
            IsCompanyAdmin = true,
            JobTitle = "Owner",
        };
        _db.CompanyMembers.Add(member);

        await _db.SaveChangesAsync(ct);

        return Result<CompanyDto>.Success(new CompanyDto(
            company.Id, company.TenantId, company.LegalName, company.TradeName,
            company.RegistrationNumber, company.Type, company.Status, company.VerificationStatus,
            company.City, company.Country, company.Website,
            company.TrustScore, company.EsgScore, company.CreatedAt));
    }
}

public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateCompanyCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(UpdateCompanyCommand request, CancellationToken ct)
    {
        var company = await _db.Companies.FirstOrDefaultAsync(c => c.Id == request.CompanyId && !c.IsDeleted, ct);
        if (company is null)
            throw new NotFoundException(nameof(Company), request.CompanyId);

        // Verify user is admin of this company
        var isAdmin = await _db.CompanyMembers
            .AnyAsync(m => m.CompanyId == request.CompanyId && m.UserId == _currentUser.UserId && m.IsCompanyAdmin, ct);
        if (!isAdmin)
            throw new ForbiddenAccessException("Only company admins can update company details.");

        if (request.TradeName is not null) company.TradeName = request.TradeName;
        if (request.AddressLine1 is not null) company.AddressLine1 = request.AddressLine1;
        if (request.AddressLine2 is not null) company.AddressLine2 = request.AddressLine2;
        if (request.City is not null) company.City = request.City;
        if (request.State is not null) company.State = request.State;
        if (request.Country is not null) company.Country = request.Country;
        if (request.PostalCode is not null) company.PostalCode = request.PostalCode;
        if (request.Phone is not null) company.Phone = request.Phone;
        if (request.Email is not null) company.Email = request.Email;
        if (request.Website is not null) company.Website = request.Website;
        if (request.BankName is not null) company.BankName = request.BankName;
        if (request.BankAccountNumber is not null) company.BankAccountNumber = request.BankAccountNumber;
        if (request.BankSwiftCode is not null) company.BankSwiftCode = request.BankSwiftCode;
        if (request.BankIban is not null) company.BankIban = request.BankIban;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class VerifyCompanyCommandHandler : IRequestHandler<VerifyCompanyCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public VerifyCompanyCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(VerifyCompanyCommand request, CancellationToken ct)
    {
        var company = await _db.Companies.FirstOrDefaultAsync(c => c.Id == request.CompanyId && !c.IsDeleted, ct);
        if (company is null)
            throw new NotFoundException(nameof(Company), request.CompanyId);

        company.VerificationStatus = request.NewStatus;
        if (request.NewStatus == VerificationStatus.Approved)
        {
            company.VerifiedAt = DateTime.UtcNow;
            company.AddDomainEvent(new CompanyVerifiedEvent(company.Id, company.TenantId));
        }

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class AddCompanyMemberCommandHandler : IRequestHandler<AddCompanyMemberCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public AddCompanyMemberCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(AddCompanyMemberCommand request, CancellationToken ct)
    {
        var isAdmin = await _db.CompanyMembers
            .AnyAsync(m => m.CompanyId == request.CompanyId && m.UserId == _currentUser.UserId && m.IsCompanyAdmin, ct);
        if (!isAdmin)
            throw new ForbiddenAccessException("Only company admins can add members.");

        var exists = await _db.CompanyMembers
            .AnyAsync(m => m.CompanyId == request.CompanyId && m.UserId == request.UserId, ct);
        if (exists)
            return Result.Failure("User is already a member of this company.");

        _db.CompanyMembers.Add(new CompanyMember
        {
            CompanyId = request.CompanyId,
            UserId = request.UserId,
            DepartmentId = request.DepartmentId,
            JobTitle = request.JobTitle,
            IsCompanyAdmin = request.IsAdmin,
        });

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class RemoveCompanyMemberCommandHandler : IRequestHandler<RemoveCompanyMemberCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public RemoveCompanyMemberCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(RemoveCompanyMemberCommand request, CancellationToken ct)
    {
        var isAdmin = await _db.CompanyMembers
            .AnyAsync(m => m.CompanyId == request.CompanyId && m.UserId == _currentUser.UserId && m.IsCompanyAdmin, ct);
        if (!isAdmin)
            throw new ForbiddenAccessException("Only company admins can remove members.");

        var member = await _db.CompanyMembers
            .FirstOrDefaultAsync(m => m.CompanyId == request.CompanyId && m.UserId == request.UserId, ct);
        if (member is null)
            return Result.Failure("Member not found.");

        _db.CompanyMembers.Remove(member);
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, Result<DepartmentDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateDepartmentCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<DepartmentDto>> Handle(CreateDepartmentCommand request, CancellationToken ct)
    {
        var company = await _db.Companies.FirstOrDefaultAsync(c => c.Id == request.CompanyId && !c.IsDeleted, ct);
        if (company is null)
            throw new NotFoundException(nameof(Company), request.CompanyId);

        var dept = new Department
        {
            CompanyId = request.CompanyId,
            Name = request.Name,
            Type = request.Type,
            ParentDepartmentId = request.ParentDepartmentId,
            ManagerUserId = request.ManagerUserId,
        };

        _db.Departments.Add(dept);
        await _db.SaveChangesAsync(ct);

        return Result<DepartmentDto>.Success(new DepartmentDto(
            dept.Id, dept.Name, dept.Type, dept.ParentDepartmentId, null));
    }
}

public class UploadCompanyDocumentCommandHandler : IRequestHandler<UploadCompanyDocumentCommand, Result<CompanyDocumentDto>>
{
    private readonly IApplicationDbContext _db;

    public UploadCompanyDocumentCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<CompanyDocumentDto>> Handle(UploadCompanyDocumentCommand request, CancellationToken ct)
    {
        var company = await _db.Companies.AnyAsync(c => c.Id == request.CompanyId && !c.IsDeleted, ct);
        if (!company)
            throw new NotFoundException(nameof(Company), request.CompanyId);

        var doc = new CompanyDocument
        {
            CompanyId = request.CompanyId,
            Type = request.Type,
            FileName = request.FileName,
            FileUrl = request.FileUrl,
            VerificationStatus = VerificationStatus.Pending,
            ExpiresAt = request.ExpiresAt,
        };

        _db.CompanyDocuments.Add(doc);
        await _db.SaveChangesAsync(ct);

        return Result<CompanyDocumentDto>.Success(new CompanyDocumentDto(
            doc.Id, doc.Type, doc.FileName, doc.FileUrl, doc.VerificationStatus, doc.ExpiresAt, doc.CreatedAt));
    }
}
