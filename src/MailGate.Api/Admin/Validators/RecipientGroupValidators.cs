using FluentValidation;
using MailGate.Contracts.RecipientGroups;

namespace MailGate.Api.Admin.Validators;

public sealed class CreateRecipientGroupValidator : AbstractValidator<CreateRecipientGroupDto>
{
    public CreateRecipientGroupValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.RecipientIds).Must(x => x.Distinct().Count() == x.Count)
            .WithMessage("RecipientIds must be unique.");
    }
}

public sealed class UpdateRecipientGroupValidator : AbstractValidator<UpdateRecipientGroupDto>
{
    public UpdateRecipientGroupValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.RecipientIds).Must(x => x.Distinct().Count() == x.Count)
            .WithMessage("RecipientIds must be unique.");
    }
}
