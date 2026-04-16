using MailGate.Channels.Smtp.Options;
using Microsoft.Identity.Client;

namespace MailGate.Channels.Smtp.Services;

public sealed class OAuth2TokenProvider(SmtpOptions options)
{
    private static readonly string[] Scopes = ["https://outlook.office365.com/.default"];

    private IConfidentialClientApplication? _app;

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (options.OAuth2 is null)
            throw new InvalidOperationException("OAuth2 is not configured.");

        _app ??= ConfidentialClientApplicationBuilder
            .Create(options.OAuth2.ClientId)
            .WithClientSecret(options.OAuth2.ClientSecret)
            .WithTenantId(options.OAuth2.TenantId)
            .Build();

        var result = await _app
            .AcquireTokenForClient(Scopes)
            .ExecuteAsync(cancellationToken);

        return result.AccessToken;
    }
}
