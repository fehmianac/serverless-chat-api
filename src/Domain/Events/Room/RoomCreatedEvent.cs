using Domain.Dto.Room;

namespace Domain.Events.Room;

public class RoomCreatedEvent
{
    public RoomDto Room { get; set; } = default!;
}