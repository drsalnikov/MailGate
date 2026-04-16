using MailGate.Channels.Smtp.Options;
using MailGate.Channels.Smtp.Services;
using MailGate.Core.Enums;
using MailGate.Core.Interfaces;
using MailGate.Core.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace MailGate.Channels.Smtp;

public sealed class SmtpChannel : INotificationChannel
{
    private readonly SmtpOptions _options;
    private readonly OAuth2TokenProvider? _tokenProvider;
    private readonly ILogger<SmtpChannel> _logger;

    public ChannelType ChannelType => ChannelType.Smtp;

    public SmtpChannel(IOptions<SmtpOptions> options, ILogger<SmtpChannel> logger)
    {
        _options = options.Value;
        _logger = logger;

        if (_options.OAuth2 is not null)
            _tokenProvider = new OAuth2TokenProvider(_options);
    }

    public async Task<ChannelResult> SendAsync(NotificationMessage message, CancellationToken cancellationToken = default)
    {
        if (message.Recipients.Count == 0)
            return ChannelResult.Skip("No recipients.");

        try
        {
            var mimeMessage = BuildMimeMessage(message);

            using var client = new SmtpClient();

            var secureSocketOptions = _options.UseSsl
                ? SecureSocketOptions.SslOnConnect
                : _options.UseTls
                    ? SecureSocketOptions.StartTls
                    : SecureSocketOptions.None;

            await client.ConnectAsync(_options.Host, _options.Port, secureSocketOptions, cancellationToken);

            if (_tokenProvider is not null)
            {
                var token = await _tokenProvider.GetAccessTokenAsync(cancellationToken);
                var oauth2 = new SaslMechanismOAuth2(
                    _options.OAuth2!.SmtpUserPrincipalName, token);
                await client.AuthenticateAsync(oauth2, cancellationToken);
            }
            else if (_options.Username is not null && _options.Password is not null)
            {
                await client.AuthenticateAsync(_options.Username, _options.Password, cancellationToken);
            }

            await client.SendAsync(mimeMessage, cancellationToken);
            await client.DisconnectAsync(quit: true, cancellationToken);

            _logger.LogInformation(
                "Email sent successfully for MessageId={MessageId}, Recipients={Count}",
                message.MessageId, message.Recipients.Count);

            return ChannelResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to send email for MessageId={MessageId}", message.MessageId);
            return ChannelResult.Failure(ex.Message);
        }
    }

    private MimeMessage BuildMimeMessage(NotificationMessage message)
    {
        var mime = new MimeMessage();
        mime.From.Add(new MailboxAddress(_options.FromName, _options.FromAddress));

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
