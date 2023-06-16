using System.Text.Json.Serialization;
using Domain.Entities.Base;

namespace Domain.Entities
{
    public class DeletedMessageEntity : IEntity
    {
        [JsonPropertyName("pk")]
        public string Pk => $"deletedMessage#{UserId}#{RoomId}";

        [JsonPropertyName("sk")]
        public string Sk => MessageId;

        [JsonPropertyName("roomId")]
        public string RoomId { get; set; } = default!;

        [JsonPropertyName("messageId")]
        public string MessageId { get; set; } = default!;

        [JsonPropertyName("userId")]
        public string UserId { get; set; } = default!;
    }
}