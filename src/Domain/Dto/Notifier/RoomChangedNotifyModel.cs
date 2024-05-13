using Domain.Dto.Room;

namespace Domain.Dto.Notifier;

public class RoomChangedNotifyModel
{
    public string EventName { get; set; } = default!;
    public string RoomId { get; set; } = default!;
    public MessageDto? Message { get; set; }
}