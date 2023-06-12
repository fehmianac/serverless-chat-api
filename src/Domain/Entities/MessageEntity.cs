using Domain.Enum;
using Domain.Extensions;

namespace Domain.Entities;

public class MessageEntity
{
    public string Pk => $"messages#{RoomId}";
    public long Sk => CreatedAt.ToUnixTimeMilliseconds();
    public string Gsi => Id;

    public string Id { get; set; } = default!;
    public string RoomId { get; set; } = default!;
    
    public string? ThreadId { get; set; }

    public string SenderId { get; set; } = default!;

    public string Body { get; set; } = default!;
    public DateTime CreatedAt { get; set; }

    public List<MessageStatusDataModel> MessageStatus { get; set; } = new();
    public List<MessageReactionDataModel> MessageReactions { get; set; } = new();
    public MessageAttachmentDataModel? MessageAttachment { get; set; }

    public class MessageStatusDataModel
    {
        public string TargetId { get; set; } = default!;
        public MessageStatus Status { get; set; }
        public DateTime CreatedUtc { get; set; }
    }

    public class MessageReactionDataModel
    {
        public string UserId { get; set; } = default!;
        public string Reaction { get; set; } = default!;
    }

    public class MessageAttachmentDataModel
    {
        public string Type { get; set; } = default!;
        public string Body { get; set; } = null!;
        public Dictionary<string, string> AdditionalData { get; set; } = new();
    }
}