using System.Text.Json.Serialization;
using Domain.Entities.Base;
using Domain.Extensions;

namespace Domain.Entities;

public class VideoCallEntity : IEntity
{
    [JsonPropertyName("pk")] public string Pk => GetPk();
    [JsonPropertyName("sk")] public string Sk => RoomId;
    [JsonPropertyName("roomId")] public string RoomId { get; set; } = default!;
    [JsonPropertyName("attendersJoined")] public List<string> AttendersJoined { get; set; } = new List<string>();
    [JsonPropertyName("startedBy")] public string? StartedBy { get; set; }
    [JsonPropertyName("startedAt")] public DateTime? StartedAt { get; set; }
    [JsonPropertyName("maxDuration")] public int MaxDuration { get; set; }
    

    [JsonPropertyName("ttl")]
    public long Ttl => (StartedAt ?? DateTime.UtcNow).AddMinutes(MaxDuration).AddMinutes(30).ToUnixTimeMilliseconds();
    public static string GetPk() => "videoCall";
}