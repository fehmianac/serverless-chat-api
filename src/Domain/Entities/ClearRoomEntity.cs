using System.Text.Json.Serialization;
using Domain.Entities.Base;

namespace Domain.Entities;

public class ClearRoomEntity : IEntity
{
    [JsonPropertyName("pk")]
    public string Pk => $"clearRoom#{UserId}";

    [JsonPropertyName("sk")]
    public string Sk => RoomId;

    [JsonPropertyName("roomId")]
    public string RoomId { get; set; } = default!;

    [JsonPropertyName("userId")]
    public string UserId { get; set; } = default!;
    
    [JsonPropertyName("time")]
    public long Time { get; set; }
}