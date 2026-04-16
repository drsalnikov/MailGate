using FluentValidation;
using MailGate.Api.Dtos;
using MailGate.Core.Enums;

namespace MailGate.Api.Validators;

public sealed class NotificationRequestValidator : AbstractValidator<NotificationRequestDto>
{
    public NotificationRequestValidator()
    {
        RuleFor(x => x.Channel)
            .IsInEnum()
            .WithMessage("Unknown channel type.");

        RuleFor(x => x.Recipients)
            .NotEmpty()
            .WithMessage("At least one recipient is required.")
            .Must(r => r.Count <= 100)
            .WithMessage("Cannot send to more than 100 recipients per request.");

        RuleForEach(x => x.Recipients)
            .NotEmpty()
            .WithMessage("Recipient address cannot be empty.")
            .When(x => x.Channel == ChannelType.Smtp, ApplyConditionTo.CurrentValidator)
            .EmailAddress()
            .WithMessage("'{PropertyValue}' is not a valid email address.");

        RuleFor(x => x.Body)
            .NotEmpty()
            .WithMessage("Body is required.")
            .MaximumLength(1_048_576)
            .WithMessage("Body exceeds the maximum allowed size of 1 MB.");

        RuleFor(x => x.Subject)
            .MaximumLength(500)
            .WithMessage("Subject cannot exceed 500 characters.")
            .When(x => x.Subject is not null);

        RuleFor(x => x.ScheduledAt)
            .GreaterThan(DateTimeOffset.UtcNow)
            .WithMessage("ScheduledAt must be in the future.")
            .When(x => x.ScheduledAt.HasValue);
    }
}
