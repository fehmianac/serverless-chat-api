using System.Text.Json.Serialization;
using Domain.Entities.Base;
using Domain.Enum;

namespace Domain.Entities
{
    public class MessageEntity : IEntity
    {
        [JsonPropertyName("pk")] public string Pk => $"messages#{RoomId}";
        [JsonPropertyName("sk")] public string Sk => Id;
        [JsonPropertyName("id")] public string Id { get; set; } = default!;
        [JsonPropertyName("roomId")] public string RoomId { get; set; } = default!;
        [JsonPropertyName("threadId")] public string? ThreadId { get; set; }
        
        [JsonPropertyName("parentId")] public string? ParentId { get; set; }
        [JsonPropertyName("senderId")] public string SenderId { get; set; } = default!;
        [JsonPropertyName("body")] public string Body { get; set; } = default!;
        [JsonPropertyName("createdAt")] public DateTime CreatedAt { get; set; }
        [JsonPropertyName("messageStatus")] public List<MessageStatusDataModel> MessageStatus { get; set; } = new();
        [JsonPropertyName("messageReactions")] public List<MessageReactionDataModel> MessageReactions { get; set; } = new();
        [JsonPropertyName("messageAttachment")] public MessageAttachmentDataModel? MessageAttachment { get; set; }

        public class MessageStatusDataModel
        {
            [JsonPropertyName("targetId")] public string TargetId { get; set; } = default!;

            [JsonPropertyName("status")] public MessageStatus Status { get; set; }

            [JsonPropertyName("createdUtc")] public DateTime CreatedUtc { get; set; }
        }

        public class MessageReactionDataModel
        {
            [JsonPropertyName("userId")] public string UserId { get; set; } = default!;

            [JsonPropertyName("reaction")] public string Reaction { get; set; } = default!;

            [JsonPropertyName("time")] public DateTime Time { get; set; }
        }

        public class MessageAttachmentDataModel
        {
            [JsonPropertyName("type")] public string Type { get; set; } = default!;

            [JsonPropertyName("payload")] public string Payload { get; set; } = null!;

            [JsonPropertyName("additionalData")] public Dictionary<string, string> AdditionalData { get; set; } = new();
        }
    }
}