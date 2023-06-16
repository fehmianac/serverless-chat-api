namespace Domain.Services;

public interface IPubSubServices
{
    Task NotifyUser(List<string> userIds, object payload, CancellationToken cancellationToken = default);
}