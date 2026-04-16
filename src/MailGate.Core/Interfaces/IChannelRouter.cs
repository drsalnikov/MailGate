using MailGate.Core.Enums;

namespace MailGate.Core.Interfaces;

public interface IChannelRouter
{
    INotificationChannel Resolve(ChannelType channelType);
    bool IsSupported(ChannelType channelType);
}
