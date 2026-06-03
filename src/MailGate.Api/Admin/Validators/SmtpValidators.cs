using FluentValidation;
using MailGate.Contracts.Smtp;

namespace MailGate.Api.Admin.Validators;

public sealed class UpdateSmtpConfigurationValidator : AbstractValidator<UpdateSmtpConfigurationDto>
{
    public UpdateSmtpConfigurationValidator()
    {
        RuleFor(x => x.Host).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Port).InclusiveBetween(1, 65535);
        RuleFor(x => x.FromAddress).NotEmpty().EmailAddress().MaximumLength(320);
        RuleFor(x => x.FromName).NotEmpty().MaximumLength(200);

        When(x => x.OAuth2 is not null, () =>
        {
            RuleFor(x => x.OAuth2!.TenantId).NotEmpty();
            RuleFor(x => x.OAuth2!.ClientId).NotEmpty();
            RuleFor(x => x.OAuth2!.SmtpUserPrincipalName).NotEmpty().EmailAddress();
        });
    }
}
