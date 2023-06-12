namespace Domain.Entities;

public class DeletedMessageEntity
{
    public string Pk => $"deletedMessage#{UserId}#{RoomId}";
    public string Sk => MessageId;

    public string RoomId { get; set; } = default!;
    public string MessageId { get; set; } = default!;
    public string UserId { get; set; } = default!;
}