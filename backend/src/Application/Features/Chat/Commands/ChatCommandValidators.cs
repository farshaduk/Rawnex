using FluentValidation;

namespace Rawnex.Application.Features.Chat.Commands;

public class CreateChatConversationCommandValidator : AbstractValidator<CreateChatConversationCommand>
{
    public CreateChatConversationCommandValidator()
    {
        RuleFor(x => x.ParticipantUserIds)
            .NotEmpty().WithMessage("At least one participant is required.")
            .Must(p => p.Count >= 1).WithMessage("At least one participant is required.");

        RuleFor(x => x.Subject)
            .MaximumLength(200).When(x => x.Subject != null);
    }
}

public class SendChatMessageCommandValidator : AbstractValidator<SendChatMessageCommand>
{
    public SendChatMessageCommandValidator()
    {
        RuleFor(x => x.ConversationId).NotEmpty();
        RuleFor(x => x.Content).NotEmpty().MaximumLength(5000);
        RuleFor(x => x.AttachmentUrl).MaximumLength(500).When(x => x.AttachmentUrl != null);
        RuleFor(x => x.AttachmentType).MaximumLength(100).When(x => x.AttachmentType != null);
    }
}

public class EditChatMessageCommandValidator : AbstractValidator<EditChatMessageCommand>
{
    public EditChatMessageCommandValidator()
    {
        RuleFor(x => x.MessageId).NotEmpty();
        RuleFor(x => x.NewContent).NotEmpty().MaximumLength(5000);
    }
}

public class DeleteChatMessageCommandValidator : AbstractValidator<DeleteChatMessageCommand>
{
    public DeleteChatMessageCommandValidator()
    {
        RuleFor(x => x.MessageId).NotEmpty();
    }
}

public class MarkChatMessagesReadCommandValidator : AbstractValidator<MarkChatMessagesReadCommand>
{
    public MarkChatMessagesReadCommandValidator()
    {
        RuleFor(x => x.ConversationId).NotEmpty();
    }
}
