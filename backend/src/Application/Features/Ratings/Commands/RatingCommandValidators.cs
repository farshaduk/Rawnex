using FluentValidation;

namespace Rawnex.Application.Features.Ratings.Commands;

public class SubmitRatingCommandValidator : AbstractValidator<SubmitRatingCommand>
{
    public SubmitRatingCommandValidator()
    {
        RuleFor(x => x.PurchaseOrderId).NotEmpty();
        RuleFor(x => x.ReviewerCompanyId).NotEmpty();
        RuleFor(x => x.ReviewedCompanyId).NotEmpty();
        RuleFor(x => x.ReviewerCompanyId).NotEqual(x => x.ReviewedCompanyId).WithMessage("Cannot rate yourself.");
        RuleFor(x => x.OverallScore).InclusiveBetween(1, 5);
        RuleFor(x => x.QualityScore).InclusiveBetween(1, 5).When(x => x.QualityScore.HasValue);
        RuleFor(x => x.DeliveryScore).InclusiveBetween(1, 5).When(x => x.DeliveryScore.HasValue);
        RuleFor(x => x.CommunicationScore).InclusiveBetween(1, 5).When(x => x.CommunicationScore.HasValue);
        RuleFor(x => x.ValueScore).InclusiveBetween(1, 5).When(x => x.ValueScore.HasValue);
        RuleFor(x => x.Comment).MaximumLength(4000);
    }
}

public class RespondToRatingCommandValidator : AbstractValidator<RespondToRatingCommand>
{
    public RespondToRatingCommandValidator()
    {
        RuleFor(x => x.RatingId).NotEmpty();
        RuleFor(x => x.ResponseComment).NotEmpty().MaximumLength(4000);
    }
}
