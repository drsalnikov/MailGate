using MailGate.Core.Enums;
using MailGate.Core.Models;

namespace MailGate.Core.Interfaces;

public interface INotificationChannel
{
    ChannelType ChannelType { get; }
    Task<ChannelResult> SendAsync(NotificationMessage message, CancellationToken cancellationToken = default);
}
