using Domain.Entities;

namespace Domain.Events.Room;

public class RoomChangedEvent
{
    public string RoomId { get; set; } = default!;
    public DateTime ActivityAt { get; set; }
    public bool HasNewMessage { get; set; }
}