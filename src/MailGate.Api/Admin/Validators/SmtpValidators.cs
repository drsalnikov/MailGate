using FluentValidation;
using MailGate.Core.Enums;
using MailGate.Contracts.Smtp;

namespace MailGate.Api.Admin.Validators;

public sealed class UpdateSmtpConfigurationValidator : AbstractValidator<UpdateSmtpConfigurationDto>
{
    public UpdateSmtpConfigurationValidator()
    {
        RuleFor(x => x.CredentialSource).Must(x => x is SmtpCredentialSource.Manual or SmtpCredentialSource.Sidecar)
            .WithMessage("Выберите ручную настройку или учётную запись из файла.");
        RuleFor(x => x).Must(x => x.CredentialSource != SmtpCredentialSource.Sidecar || x.UseTls || x.UseSsl)
            .WithMessage("Для учётной записи из файла требуется STARTTLS или SSL.");
        RuleFor(x => x.Username).Empty().WithMessage("Логин вводится только через файл учётной записи.");
        RuleFor(x => x.Password).Empty().WithMessage("Пароль вводится только через файл учётной записи.");
        RuleFor(x => x.OAuth2).Null().WithMessage("Настройка OAuth2 временно недоступна.");
        RuleFor(x => x.Host).NotEmpty().MaximumLength(256);
        RuleFor(x => x.UseSsl).Must((dto, useSsl) => !useSsl || !dto.UseTls)
            .WithMessage("Выберите один режим: STARTTLS или SSL при подключении.");
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
