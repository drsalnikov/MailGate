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
using System.Text.RegularExpressions;

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

        try
        {
            var options = await settingsProvider.GetEffectiveAsync(cancellationToken);
            logger.LogInformation(
                "SMTP credentials: UsernameConfigured={UsernameConfigured}, PasswordAvailable={PasswordAvailable}, OAuth2Configured={OAuth2Configured}",
                !string.IsNullOrWhiteSpace(options.Username), !string.IsNullOrEmpty(options.Password), options.OAuth2 is not null);
            if (options.OAuth2 is null && !string.IsNullOrWhiteSpace(options.Username) && string.IsNullOrEmpty(options.Password))
                throw new InvalidOperationException("Указан SMTP-логин, но пароль отсутствует.");
            var mimeMessage = BuildMimeMessage(message, options);

            using var client = new SmtpClient();
            client.CheckCertificateRevocation = options.CheckCertificateRevocation;

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

            logger.LogInformation("SMTP connection: IsSecure={IsSecure}, IsAuthenticated={IsAuthenticated}",
                client.IsSecure, client.IsAuthenticated);
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

        if (message.BodyFormat == BodyFormat.Html)
        {
            var builder = new BodyBuilder();
            var totalBytes = 0;
            builder.HtmlBody = Regex.Replace(message.Body,
                "src=\"(data:image/(?:png|jpeg|gif);base64,[A-Za-z0-9+/=]+)\"",
                match =>
                {
                    if (!EmbeddedEmailImage.TryRead(match.Groups[1].Value, out var mimeType, out var bytes))
                        throw new InvalidOperationException("Некорректное вложенное изображение.");
                    totalBytes += bytes.Length;
                    if (totalBytes > EmbeddedEmailImage.MaxTotalBytes)
                        throw new InvalidOperationException("Общий размер изображений превышает 512 КБ.");
                    var resource = builder.LinkedResources.Add("image", bytes, ContentType.Parse(mimeType));
                    resource.ContentId = MimeKit.Utils.MimeUtils.GenerateMessageId();
                    return $"src=\"cid:{resource.ContentId}\"";
                }, RegexOptions.CultureInvariant, TimeSpan.FromSeconds(5));
            mime.Body = builder.ToMessageBody();
        }
        else
            mime.Body = new TextPart(textFormat) { Text = message.Body };

        if (message.Priority == NotificationPriority.Critical || message.Priority == NotificationPriority.High)
            mime.Priority = MessagePriority.Urgent;

        return mime;
    }
}
