using Domain.Dto.Room;

namespace Domain.Events.Room;

public class RoomChangedEvent
{
    public string RoomId { get; set; } = default!;
    public string SenderId { get; set; } = default!;
    public DateTime ActivityAt { get; set; }
    public bool HasNewMessage { get; set; }
    public string? MessageId { get; set; }
    public MessageDto? Message { get; set; } 
}