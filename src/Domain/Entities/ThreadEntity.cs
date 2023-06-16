using System.Text.Json.Serialization;
using Domain.Extensions;

namespace Domain.Entities
{
    public class ThreadEntity
    {
        [JsonPropertyName("pk")]
        public string Pk => $"thread#{RoomId}";

        [JsonPropertyName("sk")]
        public long Sk => LastActivityAt.ToUnixTimeMilliseconds();

        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        [JsonPropertyName("roomId")]
        public string RoomId { get; set; } = default!;

        [JsonPropertyName("additionalInfo")]
        public Dictionary<string, string> AdditionalInfo { get; set; } = new();

        [JsonPropertyName("lastMessageInfo")]
        public List<LastMessageInfoDataModel>? LastMessageInfo { get; set; } = new();

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("lastActivityAt")]
        public DateTime LastActivityAt { get; set; }

        public class LastMessageInfoDataModel
        {
            [JsonPropertyName("pk")]
            public string Pk { get; set; } = default!;

            [JsonPropertyName("sk")]
            public string Sk { get; set; } = default!;
        }
    }
}