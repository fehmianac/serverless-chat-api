using Domain.Entities;

namespace Domain.Dto.Room
{
    public class RoomDto
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string ImageUrl { get; set; } = default!;
        public List<string> Attenders { get; set; } = new();
        public List<string> Admins { get; set; } = new();
        public List<MessageDto> LastMessageInfo { get; set; } = new();
        public List<TypingAttenderDto> TypingAttenders { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime LastActivityAt { get; set; }
        public int UnReadMessageCount { get; set; }
        public bool HasNotification { get; set; }
        public bool IsGroup { get; set; }

        public List<string> BannedAttenders { get; set; } = new();
        public List<string> BannerAttenders { get; set; } = new();

        public class TypingAttenderDto
        {
            public string UserId { get; set; } = default!;
            public DateTime TypingAt { get; set; }
        }
    }

    public static class RoomDtoMapper
    {
        public static RoomEntity ToEntity(this RoomDto dto)
        {
            return new RoomEntity
            {
                Admins = dto.Admins,
                Attenders = dto.Attenders,
                Description = dto.Description,
                CreatedAt = dto.CreatedAt,
                ImageUrl = dto.ImageUrl,
                TypingAttenders = dto.TypingAttenders.Select(q => new RoomEntity.TypingAttenderDataModel
                {
                    TypingAt = q.TypingAt,
                    UserId = q.UserId
                }).ToList(),
                LastActivityAt = dto.LastActivityAt,
                LastMessageInfo = dto.LastMessageInfo.Select(q => q.Id).ToList(),
                Id = dto.Id,
                Name = dto.Name
            };
        }

        public static RoomDto ToDto(this RoomEntity entity)
        {
            return new RoomDto
            {
                Admins = entity.Admins,
                Attenders = entity.Attenders,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
                ImageUrl = entity.ImageUrl,
                IsGroup = entity.IsGroup,
                TypingAttenders = entity.TypingAttenders.Select(q => new RoomDto.TypingAttenderDto
                {
                    TypingAt = q.TypingAt,
                    UserId = q.UserId
                }).ToList(),
                Id = entity.Id,
                Name = entity.Name,
                LastActivityAt = entity.LastActivityAt
            };
        }

        public static RoomDto ToDto(this RoomEntity entity, List<UserBanEntity> banEntities)
        {
            var dto = entity.ToDto();
            dto.BannedAttenders.AddRange(banEntities.Select(q => q.ToUserId));
            dto.BannerAttenders.AddRange(banEntities.Select(q => q.FromUserId));
            return dto;
        }
    }
}