using FluentValidation;

namespace Rawnex.Application.Features.Companies.Commands;

public class RegisterCompanyCommandValidator : AbstractValidator<RegisterCompanyCommand>
{
    public RegisterCompanyCommandValidator()
    {
        RuleFor(x => x.LegalName).NotEmpty().MaximumLength(300);
        RuleFor(x => x.TradeName).MaximumLength(300);
        RuleFor(x => x.RegistrationNumber).MaximumLength(100);
        RuleFor(x => x.TaxId).MaximumLength(50);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Country).MaximumLength(100);
        RuleFor(x => x.City).MaximumLength(100);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
        RuleFor(x => x.Website).MaximumLength(500);
        RuleFor(x => x.Phone).MaximumLength(20);
    }
}

public class UpdateCompanyCommandValidator : AbstractValidator<UpdateCompanyCommand>
{
    public UpdateCompanyCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
        RuleFor(x => x.Website).MaximumLength(500);
        RuleFor(x => x.Country).MaximumLength(100);
    }
}

public class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Type).IsInEnum();
    }
}
