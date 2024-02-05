using System.Text.Json.Serialization;
using Domain.Entities.Base;

namespace Domain.Entities;

public class PrivateRoomUserMappingEntity : IEntity
{
    [JsonPropertyName("pk")] public string Pk => GetPk();

    [JsonPropertyName("sk")]
    public string Sk => string.Join(',', new List<string> { UserOne, UserTwo }.OrderBy(q => q));

    [JsonPropertyName("roomId")] public string RoomId { get; set; } = default!;
    [JsonPropertyName("userOne")] public string UserOne { get; set; } = default!;
    [JsonPropertyName("userTwo")] public string UserTwo { get; set; } = default!;

    public static string GetPk()
    {
        return $"PrivateRoomUserMapping";
    }
}