using System.Text.Json.Serialization;
using Domain.Entities.Base;

namespace Domain.Entities;

public class RoomLastActivityEntity : IEntity
{
    [JsonPropertyName("pk")] public string Pk => "roomLastActivity";

    [JsonPropertyName("sk")] public string Sk => RoomId;
    [JsonPropertyName("roomId")] public string RoomId { get; set; } = default!;
    [JsonPropertyName("lastActivityAt")] public DateTime LastActivityAt { get; set; }
}