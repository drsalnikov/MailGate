using MailGate.Core.Enums;
using MailGate.Core.Interfaces;

namespace MailGate.Worker.Routing;

public sealed class ChannelRouter : IChannelRouter
{
    private readonly IReadOnlyDictionary<ChannelType, INotificationChannel> _channels;
    private readonly ILogger<ChannelRouter> _logger;

    public ChannelRouter(IEnumerable<INotificationChannel> channels, ILogger<ChannelRouter> logger)
    {
        _channels = channels.ToDictionary(c => c.ChannelType);
        _logger = logger;

        _logger.LogInformation(
            "ChannelRouter initialized with channels: {Channels}",
            string.Join(", ", _channels.Keys));
    }

    public INotificationChannel Resolve(ChannelType channelType)
    {
        if (_channels.TryGetValue(channelType, out var channel))
            return channel;

        throw new NotSupportedException(
            $"Channel '{channelType}' is not registered. Available channels: {string.Join(", ", _channels.Keys)}");
    }

    public bool IsSupported(ChannelType channelType) => _channels.ContainsKey(channelType);
}
