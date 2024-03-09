namespace Domain.Dto.Notifier;

public class RoomMessageReadModel
{
    public string RoomId { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string LastMessageId { get; set; } = default!;
    
}