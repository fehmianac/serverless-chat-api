using Domain.Events.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.Events;

public class EventPublisher : IEventPublisher
{
    private readonly IServiceProvider _serviceProvider;

    public EventPublisher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken)
    {
        var consumers = _serviceProvider.GetServices<IConsumer<T>>();
        var taskList = consumers.Select(consumer => consumer.Handler(@event, cancellationToken)).ToList();
        await Task.WhenAll(taskList);
    }
}