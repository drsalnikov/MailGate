using FluentValidation;
using MailGate.Contracts.Systems;

namespace MailGate.Api.Admin.Validators;

public sealed class CreateSystemValidator : AbstractValidator<CreateSystemDto>
{
    public CreateSystemValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(100)
            .Matches("^[a-zA-Z0-9._-]+$")
            .WithMessage("Code can contain only letters, digits, dot, underscore and dash.");

        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000);
    }
}

public sealed class UpdateSystemValidator : AbstractValidator<UpdateSystemDto>
{
    public UpdateSystemValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(100)
            .Matches("^[a-zA-Z0-9._-]+$")
            .WithMessage("Code can contain only letters, digits, dot, underscore and dash.");

        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000);
    }
}
