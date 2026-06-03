using FluentValidation;
using MailGate.Contracts.Recipients;

namespace MailGate.Api.Admin.Validators;

public sealed class CreateRecipientValidator : AbstractValidator<CreateRecipientDto>
{
    public CreateRecipientValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(320);
        RuleFor(x => x.SystemIds).Must(x => x.Distinct().Count() == x.Count)
            .WithMessage("SystemIds must be unique.");
    }
}

public sealed class UpdateRecipientValidator : AbstractValidator<UpdateRecipientDto>
{
    public UpdateRecipientValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(320);
        RuleFor(x => x.SystemIds).Must(x => x.Distinct().Count() == x.Count)
            .WithMessage("SystemIds must be unique.");
    }
}
