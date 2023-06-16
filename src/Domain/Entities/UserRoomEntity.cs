using System.Text.Json.Serialization;
using Domain.Entities.Base;
using Domain.Extensions;

namespace Domain.Entities
{
    public class UserRoomEntity : IEntity
    {
        [JsonPropertyName("pk")] public string Pk => $"userRoom#{UserId}";
        [JsonPropertyName("sk")] public string Sk => $"{LastActivityAt.ToUnixTimeMilliseconds()}#{RoomId}";
        [JsonPropertyName("userId")] public string UserId { get; set; } = default!;
        [JsonPropertyName("roomId")] public string RoomId { get; set; } = default!;
        [JsonPropertyName("lastActivityAt")] public DateTime LastActivityAt { get; set; }
    }
}