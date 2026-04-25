namespace Rawnex.Application.Common.Interfaces;

public interface IPushNotificationService
{
    Task SendToUserAsync(Guid userId, string title, string body, Dictionary<string, string>? data = null, CancellationToken ct = default);
    Task SendToTopicAsync(string topic, string title, string body, Dictionary<string, string>? data = null, CancellationToken ct = default);
}
