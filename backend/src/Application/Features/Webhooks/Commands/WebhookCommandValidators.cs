using FluentValidation;

namespace Rawnex.Application.Features.Webhooks.Commands;

public class CreateWebhookSubscriptionCommandValidator : AbstractValidator<CreateWebhookSubscriptionCommand>
{
    public CreateWebhookSubscriptionCommandValidator()
    {
        RuleFor(x => x.EventType).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Url).NotEmpty().MaximumLength(500)
            .Must(u => Uri.TryCreate(u, UriKind.Absolute, out var uri) && (uri.Scheme == "https"))
            .WithMessage("Webhook URL must be a valid HTTPS URL.");
    }
}

public class UpdateWebhookSubscriptionCommandValidator : AbstractValidator<UpdateWebhookSubscriptionCommand>
{
    public UpdateWebhookSubscriptionCommandValidator()
    {
        RuleFor(x => x.SubscriptionId).NotEmpty();
        RuleFor(x => x.EventType).MaximumLength(100).When(x => x.EventType != null);
        RuleFor(x => x.Url).MaximumLength(500)
            .Must(u => Uri.TryCreate(u, UriKind.Absolute, out var uri) && (uri.Scheme == "https"))
            .WithMessage("Webhook URL must be a valid HTTPS URL.")
            .When(x => x.Url != null);
    }
}

public class DeleteWebhookSubscriptionCommandValidator : AbstractValidator<DeleteWebhookSubscriptionCommand>
{
    public DeleteWebhookSubscriptionCommandValidator()
    {
        RuleFor(x => x.SubscriptionId).NotEmpty();
    }
}

public class TestWebhookCommandValidator : AbstractValidator<TestWebhookCommand>
{
    public TestWebhookCommandValidator()
    {
        RuleFor(x => x.SubscriptionId).NotEmpty();
    }
}
