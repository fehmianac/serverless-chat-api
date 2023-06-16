namespace Domain.Events.Contracts;

public interface IConsumer<in T>
{
    Task Handler(T payload, CancellationToken cancellationToken = default);
}