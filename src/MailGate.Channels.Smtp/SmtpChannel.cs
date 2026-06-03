using MailGate.Channels.Smtp.Options;
using MailGate.Channels.Smtp.Services;
using MailGate.Core.Enums;
using MailGate.Core.Interfaces;
using MailGate.Core.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;

namespace MailGate.Channels.Smtp;

public sealed class SmtpChannel(
    ISmtpSettingsProvider settingsProvider,
    ILogger<SmtpChannel> logger) : INotificationChannel
{
    public ChannelType ChannelType => ChannelType.Smtp;

    public async Task<ChannelResult> SendAsync(NotificationMessage message, CancellationToken cancellationToken = default)
    {
        if (message.Recipients.Count == 0)
            return ChannelResult.Skip("No recipients.");

        var options = await settingsProvider.GetEffectiveAsync(cancellationToken);

        try
        {
            var mimeMessage = BuildMimeMessage(message, options);

            using var client = new SmtpClient();

            var secureSocketOptions = options.UseSsl
                ? SecureSocketOptions.SslOnConnect
                : options.UseTls
                    ? SecureSocketOptions.StartTls
                    : SecureSocketOptions.None;

            await client.ConnectAsync(options.Host, options.Port, secureSocketOptions, cancellationToken);

            if (options.OAuth2 is not null)
            {
                var tokenProvider = new OAuth2TokenProvider(options);
                var token = await tokenProvider.GetAccessTokenAsync(cancellationToken);
                var oauth2 = new SaslMechanismOAuth2(
                    options.OAuth2.SmtpUserPrincipalName, token);
                await client.AuthenticateAsync(oauth2, cancellationToken);
            }
            else if (options.Username is not null && options.Password is not null)
            {
                await client.AuthenticateAsync(options.Username, options.Password, cancellationToken);
            }

            await client.SendAsync(mimeMessage, cancellationToken);
            await client.DisconnectAsync(quit: true, cancellationToken);

            logger.LogInformation(
                "Email sent successfully for MessageId={MessageId}, Recipients={Count}",
                message.MessageId, message.Recipients.Count);

            return ChannelResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Failed to send email for MessageId={MessageId}", message.MessageId);
            return ChannelResult.Failure(ex.Message);
        }
    }

    private static MimeMessage BuildMimeMessage(NotificationMessage message, SmtpOptions options)
    {
        var mime = new MimeMessage();
        mime.From.Add(new MailboxAddress(options.FromName, options.FromAddress));

        foreach (var recipient in message.Recipients)
            mime.To.Add(MailboxAddress.Parse(recipient));

        if (!string.IsNullOrWhiteSpace(message.Subject))
            mime.Subject = message.Subject;

        var textFormat = message.BodyFormat switch
        {
            BodyFormat.Html => TextFormat.Html,
            BodyFormat.Markdown => TextFormat.Html,
            _ => TextFormat.Plain
        };

        mime.Body = new TextPart(textFormat) { Text = message.Body };

        if (message.Priority == NotificationPriority.Critical || message.Priority == NotificationPriority.High)
            mime.Priority = MessagePriority.Urgent;

        return mime;
    }
}
