using Domain.Dto.Notifier;
using Domain.Events.Contracts;
using Domain.Events.Room;
using Domain.Services;

namespace Api.Consumers.Room;

public class RoomCreatedEventConsumer : IConsumer<RoomCreatedEvent>
{
    private readonly IPubSubServices _pubSubServices;

    public RoomCreatedEventConsumer(IPubSubServices pubSubServices)
    {
        _pubSubServices = pubSubServices;
    }

    public async Task Handler(RoomCreatedEvent payload, CancellationToken cancellationToken = default)
    {
        await _pubSubServices.NotifyUser(payload.Room.Attenders, new RoomCreatedNotifyModel
        {
            Room = payload.Room,
        }, cancellationToken);
    }
}