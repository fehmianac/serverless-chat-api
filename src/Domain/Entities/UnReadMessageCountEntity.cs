namespace Domain.Entities;

public class UnReadMessageCountEntity
{
    public string Pk => $"unReadMessageCount#{UserId}";
    public string Sk => RoomId;
    public string RoomId { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public int Count { get; set; }
}