using MailGate.Core.Models;

namespace MailGate.Core.Interfaces;

public interface INotificationDispatcher
{
    Task DispatchAsync(NotificationMessage message, CancellationToken cancellationToken = default);
}
