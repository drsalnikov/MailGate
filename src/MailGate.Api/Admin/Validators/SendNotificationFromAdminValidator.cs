using FluentValidation;
using MailGate.Contracts.Notifications;

namespace MailGate.Api.Admin.Validators;

public sealed class SendNotificationFromAdminValidator : AbstractValidator<SendNotificationFromAdminDto>
{
    public SendNotificationFromAdminValidator()
    {
        RuleFor(x => x.Body).NotEmpty().MaximumLength(1024 * 1024);
        RuleFor(x => x.Subject).MaximumLength(500);
        RuleFor(x => x.Subject).Must(x => x is null || (!x.Contains('\r') && !x.Contains('\n')))
            .WithMessage("Тема не должна содержать переводы строки.");
        RuleFor(x => x.TemplateRevision).NotNull().When(x => x.TemplateId.HasValue);
        RuleFor(x => x.Channel).Equal(MailGate.Core.Enums.ChannelType.Smtp).When(x => x.TemplateId.HasValue);

        RuleFor(x => x)
            .Must(x => x.RecipientIds.Count > 0 || x.RecipientGroupIds.Count > 0 || x.AdHocEmails.Count > 0)
            .WithMessage("Either RecipientIds, RecipientGroupIds or AdHocEmails must be provided.");

        RuleFor(x => x.RecipientIds).Must(x => x.Distinct().Count() == x.Count)
            .WithMessage("RecipientIds must be unique.");
        RuleFor(x => x.RecipientGroupIds).Must(x => x.Distinct().Count() == x.Count)
            .WithMessage("RecipientGroupIds must be unique.");

        RuleForEach(x => x.AdHocEmails).EmailAddress();
    }
}
