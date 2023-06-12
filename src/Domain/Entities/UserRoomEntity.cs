using Domain.Extensions;

namespace Domain.Entities;

public class UserRoomEntity
{
    public string Pk => $"userRoom#{UserId}";
    public long Sk => LastActivityAt.ToUnixTimeMilliseconds();
    public string Gsi => RoomId;
    public string UserId { get; set; } = default!;
    public string RoomId { get; set; } = default!;
    public DateTime LastActivityAt { get; set; }
}