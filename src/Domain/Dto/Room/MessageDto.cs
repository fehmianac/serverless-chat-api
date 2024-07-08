using Domain.Entities;
using Domain.Enum;

namespace Domain.Dto.Room
{
    public class MessageDto
    {
        public string Id { get; set; } = default!;

        public string RoomId { get; set; } = default!;

        public string? ThreadId { get; set; }

        public string SenderId { get; set; } = default!;

        public string Body { get; set; } = default!;

        public DateTime CreatedAt { get; set; }

        public List<MessageStatusDto> MessageStatus { get; set; } = new();

        public List<MessageReactionDto> MessageReactions { get; set; } = new();


        public MessageAttachmentDto? MessageAttachment { get; set; }
        public bool IsDeleted { get; set; }
        public MessageDto? Parent { get; set; }
        public class MessageStatusDto
        {
            public string TargetId { get; set; } = default!;

            public MessageStatus Status { get; set; }

            public DateTime CreatedUtc { get; set; }
        }

        public class MessageReactionDto
        {
            public string UserId { get; set; } = default!;
            public string Reaction { get; set; } = default!;

            public DateTime Time { get; set; }
        }

        public class MessageAttachmentDto
        {
            public string Type { get; set; } = default!;

            public string Payload { get; set; } = null!;

            public Dictionary<string, string> AdditionalData { get; set; } = new();
            public bool IsSensitive { get; set; }
        }
    }

    public static class MessageDtoMapper
    {
        public static MessageEntity ToEntity(this MessageDto dto)
        {
            return new MessageEntity
            {
            };
        }

        public static MessageDto ToDto(this MessageEntity entity)
        {
            return new MessageDto
            {
                Body = entity.Body,
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                Parent = entity.ParentId == null ? null : new MessageDto
                {
                    Id = entity.ParentId
                },
                MessageAttachment = entity.MessageAttachment == null
                    ? null
                    : new MessageDto.MessageAttachmentDto
                    {
                        Payload = entity.MessageAttachment.Payload,
                        AdditionalData = entity.MessageAttachment.AdditionalData,
                        Type = entity.MessageAttachment.Type
                    },
                MessageReactions = entity.MessageReactions.Select(q => new MessageDto.MessageReactionDto
                {
                    Reaction = q.Reaction,
                    Time = q.Time,
                    UserId = q.UserId
                }).ToList(),
                MessageStatus = entity.MessageStatus.Select(q => new MessageDto.MessageStatusDto
                {
                    Status = q.Status,
                    CreatedUtc = q.CreatedUtc,
                    TargetId = q.TargetId
                }).ToList(),
                RoomId = entity.RoomId,
                SenderId = entity.SenderId,
                ThreadId = entity.ThreadId
            };
        }
    }
}