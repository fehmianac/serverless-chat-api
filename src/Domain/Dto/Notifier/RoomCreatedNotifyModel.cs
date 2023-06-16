using Domain.Dto.Room;

namespace Domain.Dto.Notifier;

public class RoomCreatedNotifyModel
{
    public string EventName =>"RoomCreated";
    public RoomDto Room { get; set; } = default!;
}