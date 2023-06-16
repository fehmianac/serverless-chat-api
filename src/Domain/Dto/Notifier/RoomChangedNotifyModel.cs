namespace Domain.Dto.Notifier;

public class RoomChangedNotifyModel
{
    public string EventName { get; set; } = default!;
    public string RoomId { get; set; } = default!;
}