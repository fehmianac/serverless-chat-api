using System.Text.Json.Serialization;
using Domain.Entities.Base;
using Domain.Extensions;

namespace Domain.Entities
{
    public class RoomNotificationEntity : IEntity
    {
        [JsonPropertyName("pk")] public string Pk => $"roomNotification#{UserId}";
        [JsonPropertyName("sk")] public string Sk => RoomId;
        [JsonPropertyName("roomId")] public string RoomId { get; set; } = default!;
        [JsonPropertyName("userId")] public string UserId { get; set; } = default!;
        [JsonPropertyName("count")] public int MessageCount { get; set; }
        [JsonPropertyName("hasNotification")] public bool HasNotification { get; set; }
        [JsonPropertyName("messageIds")] public List<string> MessageIds { get; set; } = new();


        [JsonPropertyName("ttl")]
        public long Ttl
        {
            get
            {
                if (MessageCount == 0)
                {
                    return DateTime.UtcNow.AddMinutes(10).ToUnixTimeMilliseconds();
                }

                return DateTime.UtcNow.AddDays(7).ToUnixTimeMilliseconds();
            }
        }
    }
}